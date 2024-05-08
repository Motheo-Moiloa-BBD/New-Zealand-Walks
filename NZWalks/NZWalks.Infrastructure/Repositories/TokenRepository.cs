using Microsoft.AspNetCore.Identity;
using NZWalks.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Infrastructure.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        public string CreateToken(IdentityUser user, List<string> roles)
        {
           //Create Claims
        }
    }
}
