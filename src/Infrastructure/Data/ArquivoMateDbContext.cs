using ArquivoMate.Domain.Entities;
using ArquivoMate.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Data
{
    //    dotnet ef migrations add InitialCreate --context PostgreSqlArquivoMateDbContext --output-dir Migrations/PostgreSqlMigrations
    //    dotnet ef database update --context PostgreSqlArquivoMateDbContext

    public class ArquivoMateDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        //public DbSet<Domain.Entities.Document> Documents { get; set; }

        //public DbSet<Tag> Tags { get; set; }

        //public DbSet<DocumenTag> DocumenTags { get; set; }      

    }
}
