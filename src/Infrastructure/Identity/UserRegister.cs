﻿using ArquivoMate.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArquivoMate.Infrastructure.Identity
{
    public partial class UserService
    {

        public async Task<AppResponse<bool>> UserRegisterAsync(UserRegisterRequest request)
        {
            var user = new ApplicationUser()
            {
                UserName = request.Email,
                Email = request.Email,

            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                return new AppResponse<bool>().SetSuccessResponse(true);
            }
            else
            {
                return new AppResponse<bool>().SetErrorResponse(GetRegisterErrors(result));
            }
        }

        private Dictionary<string, string[]> GetRegisterErrors(IdentityResult result)
        {
            var errorDictionary = new Dictionary<string, string[]>(1);

            foreach (var error in result.Errors)
            {
                string[] newDescriptions;

                if (errorDictionary.TryGetValue(error.Code, out var descriptions))
                {
                    newDescriptions = new string[descriptions.Length + 1];
                    Array.Copy(descriptions, newDescriptions, descriptions.Length);
                    newDescriptions[descriptions.Length] = error.Description;
                }
                else
                {
                    newDescriptions = [error.Description];
                }

                errorDictionary[error.Code] = newDescriptions;
            }

            return errorDictionary;
        }
    }
}
