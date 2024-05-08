using Microsoft.AspNetCore.Identity;
using NZWalks.Core.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Services.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityUser> RegisterUser(RegisterDTO registerDTO);
        Task LoginUser(LoginDTO loginDTO);
    }
}
