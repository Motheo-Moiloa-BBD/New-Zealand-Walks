using Microsoft.AspNetCore.Identity;
using NZWalks.Core.Interfaces;
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
        private readonly IUnitOfWork unitOfWork;

        public AuthService(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
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
                if(registerDTO.Roles != null)
                {
                    identityResult = await userManager.AddToRolesAsync(identityUser, registerDTO.Roles);

                    if (identityResult.Succeeded)
                    {
                        return identityUser;
                    }
                }
                
            }

            return null;
        }

        public async Task<LoginResponseDTO> LoginUser(LoginDTO loginDTO)
        {
            var user = await userManager.FindByEmailAsync(loginDTO.Username);

            if(user != null)
            {
               var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginDTO.Password);

                if (checkPasswordResult)
                {
                    //Get roles for this user
                    var roles = await userManager.GetRolesAsync(user);

                    if(roles != null)
                    {
                        //Create token
                        var token = unitOfWork.Tokens.CreateToken(user, roles.ToList());

                        var response = new LoginResponseDTO
                        {
                            JwtToken = token
                        };

                        return response;
                    }
                   
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
