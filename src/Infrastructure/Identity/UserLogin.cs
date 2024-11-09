using ArquivoMate.Application;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Identity
{
    public class UserLoginRequest
    {
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
    public class UserLoginResponse
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
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
