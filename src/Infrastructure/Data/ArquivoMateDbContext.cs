using ArquivoMate.Application.Interfaces;
using ArquivoMate.Domain.Common;
using ArquivoMate.Domain.Entities;
using ArquivoMate.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Data
{
    //    dotnet ef migrations add InitialCreate --context PostgreSqlArquivoMateDbContext --output-dir Migrations/PostgreSqlMigrations
    //    dotnet ef database update --context PostgreSqlArquivoMateDbContext

    public class ArquivoMateDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        protected readonly Lazy<IUserService> userService;

        public ArquivoMateDbContext(Lazy<IUserService> userService)
        {
            this.userService = userService;
        }

        public DbSet<Domain.Entities.Document> Documents { get; set; } = null!;

        public DbSet<Tag> Tags { get; set; } = null!;

        public DbSet<TagDocument> DocumenTags { get; set; } = null!;

        public DbSet<DocumentVersion> DocumentVersions { get; set; } = null!; 

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void HandleChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if ((entry.State is EntityState.Added) && (entry.Entity.GetType().IsSubclassOfGeneric(typeof(BaseAuditableEntity<>))))
                {
                    entry.Property("Owner").CurrentValue = userService.Value.GetUserId() ?? Guid.Empty;
                    entry.Property("LastModified").CurrentValue = DateTime.UtcNow;
                    entry.Property("LastModifiedBy").CurrentValue = userService.Value.GetUserName();

                    if (entry.State == EntityState.Added)
                    {
                        entry.Property("Created").CurrentValue = DateTime.UtcNow;
                        entry.Property("CreatedBy").CurrentValue = userService.Value.GetUserName();
                    }
                }

                if (entry.State == EntityState.Deleted && (entry.Entity.GetType() is ISoftDelete))
                {
                    entry.State = EntityState.Modified;
                    entry.Property("IsDeleted").CurrentValue = true;
                }
            }
        }


        protected void ApplyGlobalFilters(ModelBuilder builder)
        {
            var entities = GetEntities(builder);

            foreach (var entity in entities)
            {
                var baseEntityType = entity.BaseType;
                if (baseEntityType != null && baseEntityType.IsGenericType && baseEntityType.GetGenericTypeDefinition() == typeof(BaseEntity<>))
                {
                    builder.Entity(entity).Property("Id").ValueGeneratedOnAdd();
                }
            }

            Expression<Func<BaseAuditableEntity<Guid>, bool>> filterExpressionTenant = (entity) => entity.Owner == userService.Value.GetUserId();
            entities = GetEntities<BaseAuditableEntity<Guid>>(builder);
            ApplySingleFilter(entities, filterExpressionTenant, builder);
            ApplyIndex(entities, nameof(BaseAuditableEntity<Guid>.Owner), builder, (index) => { });

            Expression<Func<ISoftDelete, bool>> filterExpressionSoftDelete = (entity) => entity.IsDeleted == false;
            entities = GetEntities<ISoftDelete>(builder);
            ApplySingleFilter(entities, filterExpressionSoftDelete, builder);
            ApplyIndex(entities, nameof(ISoftDelete.IsDeleted), builder, (index) => { });
        }

        private IList<Type> GetEntities<TInterface>(ModelBuilder modelBuilder)
        {
            var entities = modelBuilder.Model
             .GetEntityTypes()
             .Where(t => t.BaseType == null)
             .Select(t => t.ClrType)
             .Where(t => typeof(TInterface).IsAssignableFrom(t)).ToList();

            return entities;
        }

        private void ApplyIndex(IList<Type> entities, string fieldName, ModelBuilder modelBuilder, Action<IndexBuilder> action)
        {
            foreach (var entity in entities)
            {
                action(
                    modelBuilder.Entity(entity).HasIndex(new string[] { fieldName }));
            }
        }

        private void ApplySingleFilter<TInterface>(IList<Type> entities, Expression<Func<TInterface, bool>> expression, ModelBuilder modelBuilder)
        {
            foreach (var entity in entities)
            {
                var newParam = Expression.Parameter(entity);
                var newbody = ReplacingExpressionVisitor.Replace(expression.Parameters.Single(), newParam, expression.Body);
                modelBuilder.Entity(entity).HasQueryFilter(Expression.Lambda(newbody, newParam));
            }
        }

        private IList<Type> GetEntities(ModelBuilder modelBuilder)
        {
            var entities = modelBuilder.Model
             .GetEntityTypes()
             .Where(t => t.BaseType == null)
             .Select(t => t.ClrType)
             .Where(type => type.IsSubclassOfGeneric(typeof(BaseEntity<>))).ToList();

            return entities;
        }


    }


    public static class DbContextExtensions
    {
        public static bool IsSubclassOfGeneric(this Type type, Type genericBaseType)
        {
            while (type != null && type != typeof(object))
            {
                var currentType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (currentType == genericBaseType)
                    return true;
                type = type.BaseType;
            }
            return false;
        }
    }

}
