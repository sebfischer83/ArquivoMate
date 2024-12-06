
using ArquivoMate.Infrastructure;
using ArquivoMate.Infrastructure.Data;
using ArquivoMate.Infrastructure.Identity;
using ArquivoMate.Infrastructure.Services.Communication;
using ArquivoMate.Infrastructure.Settings.FileProvider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Serilog;

namespace ArquivoMate.WebApi
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddEnvironmentVariables("AMate__");
            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));

            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //builder.Services.AddAuthorization(options =>
            //{
            //    options.FallbackPolicy = new AuthorizationPolicyBuilder()
            //        .RequireAuthenticatedUser()
            //        .Build();
            //});

            var app = builder.Build();
            app.UseMiddleware<CheckTokenMiddleware>();
            FileProviderSettingsFactory fileProviderSettingsFactory;

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ArquivoMateDbContext>();
                    fileProviderSettingsFactory = services.GetRequiredService<FileProviderSettingsFactory>();

                    context.Database.Migrate();
                    await CreateRolesAndAdminUser(services);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            if (fileProviderSettingsFactory.GetFileProviderSettings().Type == FileProviderType.Local)
            {
                app.UseStaticFiles();
            }
            app.UseSerilogRequestLogging();
            app.UseAuthentication();
            app.UseAuthorization();

            if (fileProviderSettingsFactory.GetFileProviderSettings().Type == FileProviderType.Local)
            {
                var fileProviderSettings = fileProviderSettingsFactory.GetFileProviderSettings() as LocalFileProviderSettings;
                if (fileProviderSettings == null)
                {
                    throw new InvalidOperationException("FileProvider settings are not configured.");
                }
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(fileProviderSettings.Path),
                    RequestPath = fileProviderSettings.RequestPath
                });
            }

            app.MapControllers();
            app.MapHub<SignalRCommunicationHub>("/status", opt =>
            {
                opt.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets |
                    Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
            });

            app.Run();
        }

        static async Task CreateRolesAndAdminUser(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Admin", "User", "Manager" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new ApplicationRole(roleName));
                }
            }

            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByNameAsync("admin");

            if (adminUser == null)
            {
                var newAdminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail
                };

                string adminPassword = "Admin1234%%";
                var createAdminResult = await userManager.CreateAsync(newAdminUser, adminPassword);

                if (createAdminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdminUser, "Admin");
                }
            }
        }
    }

}
