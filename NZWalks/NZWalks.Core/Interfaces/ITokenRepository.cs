using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Core.Interfaces
{
    public interface ITokenRepository
    {
        //Add methods that are specific to the Token entity 
        string CreateToken(IdentityUser user, List<string> roles);
    }
}
