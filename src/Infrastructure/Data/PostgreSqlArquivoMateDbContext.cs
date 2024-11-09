using ArquivoMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Data
{
    public class PostgreSqlArquivoMateDbContext(IConfiguration configuration) : ArquivoMateDbContext
    {
        private readonly IConfiguration configuration = configuration;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(configuration["Database:ConnectionString"]);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasPostgresExtension("pg_trgm");
            builder.HasPostgresExtension("intarray");


            //builder.Entity<Document>()
            //.HasMany(e => e.Tags)
            //.WithMany(e => e.Documents)
            //.UsingEntity<DocumenTag>(
            //    l => l.HasOne<Tag>(e => e.Tag).WithMany(e => e.DocumenTags),
            //    r => r.HasOne<Document>(e => e.Document).WithMany(e => e.DocumentTags));
        }
    }

    public class PostgreSqlArquivoMateDbContextFactory : IDesignTimeDbContextFactory<PostgreSqlArquivoMateDbContext>
    {
        public PostgreSqlArquivoMateDbContext CreateDbContext(string[] args)
        {
            Dictionary<string, string?> configurationDictionary = new Dictionary<string, string?>
            {
                { "Database:ConnectionString", 
                    "Host=localhost;Port=5432;Database=arquivomate;Username=root;Password=lF18xggsdf4325d3z0mmN" }
            };

            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationDictionary.ToArray())
                .Build();

            return new PostgreSqlArquivoMateDbContext(configuration);
        }
    }
}
