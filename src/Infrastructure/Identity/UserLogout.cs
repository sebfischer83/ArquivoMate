﻿using ArquivoMate.Application;
using ArquivoMate.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Identity
{  

    public partial class UserService
    {
        public async Task<AppResponse<bool>> UserLogoutAsync(UserLogoutRequest request, ClaimsPrincipal user)
        {
            var database = connectionMultiplexer.GetDatabase();
            database.StringSet($"Revoke:{request.AccessToken}", "true");
            database.StringSet($"Revoke:{request.RefreshToken}", "true");

            if (user.Identity?.IsAuthenticated ?? false)
            {
                var username = user.Claims.First(x => x.Type == "UserName").Value;
                var appUser = _context.Users.First(x => x.UserName == username);
                if (appUser != null) { await _userManager.UpdateSecurityStampAsync(appUser); }

                return new AppResponse<bool>().SetSuccessResponse(true);
            }

            return new AppResponse<bool>().SetSuccessResponse(true);
        }
    }
}
