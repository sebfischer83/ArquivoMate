
using ArquivoMate.Infrastructure;
using ArquivoMate.Infrastructure.Data;
using ArquivoMate.Infrastructure.Identity;
using ArquivoMate.Infrastructure.Services.Communication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArquivoMate.WebApi
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            //app.MapIdentityApi<ApplicationUser>();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ArquivoMateDbContext>();

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

            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<DocumentStatusHub>("/documentStatus", opt =>
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
