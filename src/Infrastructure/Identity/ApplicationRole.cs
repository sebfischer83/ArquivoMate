using Microsoft.AspNetCore.Identity;

namespace ArquivoMate.Infrastructure.Identity
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public ApplicationRole(string roleName) : base(roleName)
        {
            
        }

        public ApplicationRole() : base()
        {
            
        }
    }
}
