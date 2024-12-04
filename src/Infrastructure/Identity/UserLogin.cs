using ArquivoMate.Application;
using ArquivoMate.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Identity
{   
    public partial class UserService
    {
        public async Task<AppResponse<UserLoginResponse>> UserLoginAsync(UserLoginRequest request)
        {
            ApplicationUser? user = null;
            if (!string.IsNullOrWhiteSpace(request.UserName))
            {
                user = await _userManager.FindByNameAsync(request.UserName);
            }
            else
            {
                user = await _userManager.FindByEmailAsync(request.Email);
            }
            if (user == null)
            {

                return new AppResponse<UserLoginResponse>().SetErrorResponse("email", "User not found");
            }
            else
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);
                if (result.Succeeded)
                {
                    var token = await GenerateUserToken(user);
                    return new AppResponse<UserLoginResponse>().SetSuccessResponse(token);
                }
                else
                {
                    return new AppResponse<UserLoginResponse>().SetErrorResponse("password", result.ToString());
                }
            }
        }

    }
}
