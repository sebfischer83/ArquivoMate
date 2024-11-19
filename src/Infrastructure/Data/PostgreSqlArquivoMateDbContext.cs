using ArquivoMate.Application.Interfaces;
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
    public class PostgreSqlArquivoMateDbContext(IConfiguration configuration, Lazy<IUserService> userService) : ArquivoMateDbContext(userService)
    {
        private readonly IConfiguration configuration = configuration;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(configuration["Database:ConnectionString"]);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasPostgresExtension("pg_trgm");
            builder.HasPostgresExtension("intarray");
            base.OnModelCreating(builder);

            builder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FilePath).IsRequired();
                entity.Property(e => e.OriginalFileName).HasMaxLength(250);
                entity.Property(e => e.OriginalFilePath).HasMaxLength(2000);
                entity.Property(e => e.FileExtension).HasMaxLength(20);
                entity.Property(e => e.FileSize);
                entity.Property(e => e.ThumbnailImage).HasMaxLength(2000);
                entity.Property(e => e.FullImage).HasMaxLength(2000);
                entity.Property(e => e.PreviewFile).HasMaxLength(1000);
                entity.Property(e => e.Content);
                entity.Property(e => e.GeneratedContent);
                entity.Property(e => e.Comment).HasMaxLength(1000);
                entity.Property(e => e.DocumentDate);
                entity.HasMany(e => e.Tags).WithMany(t => t.Documents).UsingEntity<TagDocument>();
                entity.HasMany(e => e.Versions).WithOne(v => v.Document);
            });

            builder.Entity<Tag>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.HasMany(e => e.Documents).WithMany(d => d.Tags).UsingEntity<TagDocument>();
            });

            builder.Entity<DocumentVersion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OriginalFilePath).HasMaxLength(2000).IsRequired();
                entity.HasOne(e => e.Document).WithMany(d => d.Versions).HasForeignKey(e => e.DocumentId);
            });

            ApplyGlobalFilters(builder);
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

            return new PostgreSqlArquivoMateDbContext(configuration, null);
        }
    }
}
