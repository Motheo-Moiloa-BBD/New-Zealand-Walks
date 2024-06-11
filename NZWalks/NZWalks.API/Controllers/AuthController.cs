using Microsoft.AspNetCore.Mvc;
using NZWalks.Core.Models.DTO;
using NZWalks.Services.Interfaces;

namespace NZWalks.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var result = await authService.RegisterUser(registerDTO);
            
            return Ok("User was registered successfuly.");
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await authService.LoginUser(loginDTO);

            return Ok(result);
        }
    }
}
