
using AuctionHouse.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuctionHouse.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        // DTOs moved to Models/AuthRequests.cs

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuctionHouse.Api.Models.RegisterRequest model)
        {
            var token = await _authService.RegisterAsync(model.Username, model.Email, model.Password);
            if (token == null)
                return BadRequest(new { message = "Registration failed" });
            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuctionHouse.Api.Models.LoginRequest model)
        {
            var token = await _authService.LoginAsync(model.Email, model.Password);
            if (token == null)
                return Unauthorized(new { message = "Invalid credentials" });
            return Ok(new { token });
        }
    }
}
