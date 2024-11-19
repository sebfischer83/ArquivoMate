using ArquivoMate.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Identity
{
    public class UserDataService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> userManager;

        public UserDataService(IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public Guid? GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                return null;
            }
            var userIdClaim = user.FindFirst("Id")?.Value;
            if (userIdClaim == null)
            {
                return null;
            }

            return Guid.Parse(userIdClaim);
        }


        public string? GetUserName()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                return null; // Kein Benutzer authentifiziert
            }

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }
    }
}
