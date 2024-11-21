using ArquivoMate.Application;
using ArquivoMate.Infrastructure.Identity;
using ArquivoMate.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static MassTransit.ValidationResultExtensions;

namespace ArquivoMate.WebApi.Controllers.User
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController(UserService userService) : ControllerBase
    {
        private readonly UserService _userService = userService;

        [HttpPost]
        [ProducesResponseType(typeof(AppResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AppResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(UserRegisterRequest req)
        {
            var result = await _userService.UserRegisterAsync(req);
            if (result.IsSucceed)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(AppResponse<UserLoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AppResponse<UserLoginResponse>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(UserLoginRequest req)
        {
            var result = await _userService.UserLoginAsync(req);
            if (result.IsSucceed)
            {
                return Ok(result);
            }
            else
            {
                return Unauthorized(result);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(AppResponse<UserRefreshTokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AppResponse<UserRefreshTokenResponse>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken(UserRefreshTokenRequest req)
        {
            var result = await _userService.UserRefreshTokenAsync(req);
            if (result.IsSucceed)
            {
                return Ok(result);
            }
            else
            {
                return Unauthorized(result);
            }
        }
        [HttpPost]
        [ProducesResponseType(typeof(AppResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AppResponse<bool>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout(UserLogoutRequest userLogout)
        {
            var result = await _userService.UserLogoutAsync(userLogout, User);
            if (result.IsSucceed)
            {
                return Ok(result);
            }
            else
            {
                return Unauthorized(result);
            }
        }

        [HttpPost]
        [Authorize]
        public string Profile()
        {
            return User.FindFirst("UserName")?.Value ?? "";
           
        }
    }
}
