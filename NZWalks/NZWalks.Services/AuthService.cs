using Microsoft.AspNetCore.Identity;
using NZWalks.Core.Models.DTO;
using NZWalks.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> userManager;

        public AuthService(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<IdentityUser> RegisterUser(RegisterDTO registerDTO)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerDTO.Username,
                Email = registerDTO.Username
            };

            var identityResult = await userManager.CreateAsync(identityUser, registerDTO.Password);

            if(identityResult.Succeeded)
            {
                //Add roles to this user
                if(registerDTO.Roles != null && registerDTO.Roles.Any())
                identityResult = await userManager.AddToRolesAsync(identityUser, registerDTO.Roles);

                if (identityResult.Succeeded)
                {
                    return identityUser;
                }

            }

            return null;
        }

        public async Task LoginUser(LoginDTO loginDTO)
        {
            var user = await userManager.FindByEmailAsync(loginDTO.Username);

            if(user != null)
            {
               var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginDTO.Password);

                if (checkPasswordResult)
                {
                    //Create token
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

    }
}
