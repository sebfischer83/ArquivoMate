using ArquivoMate.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Guid?> GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                return null;
            }
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return null;
            }
            var appUser = await userManager.FindByIdAsync(userIdClaim);
            return appUser?.Id;
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
