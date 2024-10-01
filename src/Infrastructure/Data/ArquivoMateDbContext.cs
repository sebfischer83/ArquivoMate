using ArquivoMate.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Data
{
    // dotnet ef migrations add InitialCreate --context BlogContext --output-dir Migrations/SqlServerMigrations
    public class ArquivoMateDbContext : IdentityDbContext<ApplicationUser>
    {

    }
}
