using ArquivoMate.Application;
using ArquivoMate.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArquivoMate.WebApi.Controllers.User
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController(UserService userService) : ControllerBase
    {
        private readonly UserService _userService = userService;

        [HttpPost]
        public async Task<AppResponse<bool>> Register(UserRegisterRequest req)
        {
            return await _userService.UserRegisterAsync(req);
        }

        [HttpPost]
        public async Task<AppResponse<UserLoginResponse>> Login(UserLoginRequest req)
        {
            return await _userService.UserLoginAsync(req);
        }

        [HttpPost]
        public async Task<AppResponse<UserRefreshTokenResponse>> RefreshToken(UserRefreshTokenRequest req)
        {
            return await _userService.UserRefreshTokenAsync(req);
        }
        [HttpPost]
        public async Task<AppResponse<bool>> Logout(UserLogoutRequest userLogout)
        {
            return await _userService.UserLogoutAsync(userLogout, User);
        }

        [HttpPost]
        [Authorize]
        public string Profile()
        {
            return User.FindFirst("UserName")?.Value ?? "";
        }
    }
}
