using ArquivoMate.Application.Document;
using ArquivoMate.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ArquivoMate.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
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

            services.AddMassTransit(x =>
            {
                x.AddConsumer<DocumentAddedConsumer>(c =>
                {
                    c.ConcurrentMessageLimit = 4;
                });

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq", "/", c =>
                    {
                        c.Username("guest");
                        c.Password("guest");
                    });

                    cfg.ReceiveEndpoint("image-processing-queue", e =>
                    {
                        e.ConfigureConsumer<DocumentAddedConsumer>(context);
                    });
                });
            });

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
