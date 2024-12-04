using ArquivoMate.Application.Commands.Document;
using ArquivoMate.Application.Interfaces;
using ArquivoMate.Infrastructure.Data;
using ArquivoMate.Infrastructure.Identity;
using ArquivoMate.Infrastructure.Services.Communication;
using ArquivoMate.Infrastructure.Services.Consumer;
using ArquivoMate.Infrastructure.Services.Document;
using ArquivoMate.Infrastructure.Services.Files;
using ArquivoMate.Infrastructure.Services.MessageQueue;
using Blobject.AmazonS3;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using StackExchange.Redis;
using System.Text;

namespace ArquivoMate.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // file provider
            services.AddSingleton<FileProviderSettingsFactory>();
                       
            // ef core
            DatabaseConfigurations databaseConfigurations = new DatabaseConfigurations();
            configuration.GetSection("Database").Bind(databaseConfigurations);

            switch (databaseConfigurations.Type)
            {
                case DatabaseType.PostgreSql:
                    services.AddDbContext<ArquivoMateDbContext, PostgreSqlArquivoMateDbContext>();
                    break;
                case DatabaseType.SqlServer:
                    break;
                case DatabaseType.SQLLite:
                    break;
                case DatabaseType.MariaDB:
                    break;
            }

            // mediatr
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
                config.RegisterServicesFromAssembly(typeof(EnqueueDocumentCommand).Assembly);
            });

            // mass transit
            services.AddMassTransit(x =>
            {
                x.AddConsumer<EnqueueDocumentConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint(Constants.RabbitMqQueueName, e =>
                    {
                        e.ConfigureConsumer<EnqueueDocumentConsumer>(context);
                    });
                });
            });

            services.AddScoped<IMessageQueue<EnqueueDocumentCommand>,
                Services.MessageQueue.MassTransitMessageQueue<EnqueueDocumentCommand>>();

            // polly
            services.AddHttpClient("ocrmypdf", client =>
            {
                client.BaseAddress = new Uri(configuration["DocumentProcessors:OcrMyPdf-Server"]);
            });
            services.AddHttpClient("tesseract", client =>
            {
                client.BaseAddress = new Uri(configuration["DocumentProcessors:Tesseract-Server"]);
            });
            //services.AddHttpClient("gotenberg", client =>
            //{
            //    client.BaseAddress = new Uri(configuration["Gotenberg:OcrMyPdf-Server"]);
            //});

            // signalR
            services.AddSignalR().AddJsonProtocol();

            // services
            services.AddScoped<IDocumentProcessor, DocumentProcessor>();
            services.AddHttpContextAccessor();
            services.AddScoped<IUserService, UserDataService>();
            services.AddScoped<Lazy<IUserService>>(provider => new Lazy<IUserService>(() => provider.GetRequiredService<IUserService>()));
            services.AddScoped<ICommunicationHub, SignalRCommunicationService>();
            services.AddScoped<IFileService, FileService>();

            // authentication
            var appSettings = configuration.GetSection("TokenSettings").Get<TokenSettings>() ?? default!;
            services.AddSingleton(appSettings);

            services.AddIdentityCore<ApplicationUser>()
                 .AddRoles<ApplicationRole>()
                 .AddSignInManager()
                 .AddEntityFrameworkStores<ArquivoMateDbContext>()
                 .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("REFRESHTOKENPROVIDER");

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromSeconds(appSettings.RefreshTokenExpireSeconds);
            });
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    ValidIssuer = appSettings.Issuer,
                    ValidAudience = appSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.SecretKey)),
                    ClockSkew = TimeSpan.FromSeconds(0)
                };
            });
            services.AddTransient<UserService>();

            // redis
            string redisServer = configuration["Cache:RedisServer"]!;
            string redisPort = configuration["Cache:RedisPort"]!;
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect($"{redisServer}:{redisPort}"));

            return services;
        }
    }

    public class DatabaseConfigurations
    {
        public string ConnectionString { get; set; } = null!;
        public DatabaseType Type { get; set; }
    }

    public enum DatabaseType
    {
        PostgreSql,
        SqlServer,
        SQLLite,
        MariaDB
    }
}
