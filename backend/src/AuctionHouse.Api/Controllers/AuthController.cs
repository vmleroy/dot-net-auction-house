
using AuctionHouse.Application.Interfaces;
using AuctionHouse.Application.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuctionHouse.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public AuthController(IAuthService authService, IRefreshTokenService refreshTokenService, JwtTokenGenerator jwtTokenGenerator)
        {
            _authService = authService;
            _refreshTokenService = refreshTokenService;
            _jwtTokenGenerator = jwtTokenGenerator;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuctionHouse.Api.Models.RegisterRequest model)
        {
            try
            {
                var user = await _authService.RegisterUserAsync(model.Username, model.Email, model.Password);
                
                var token = _jwtTokenGenerator.GenerateToken(user);
                var refreshToken = await _refreshTokenService.GenerateAsync(user.Id);

                return Ok(new AuctionHouse.Api.Models.RefreshTokenResponse { Token = token, RefreshToken = refreshToken.Token });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuctionHouse.Api.Models.LoginRequest model)
        {
            try
            {
                var user = await _authService.ValidateUserAsync(model.Email, model.Password);

                var token = _jwtTokenGenerator.GenerateToken(user);
                var refreshToken = await _refreshTokenService.GenerateAsync(user.Id);

                return Ok(new AuctionHouse.Api.Models.RefreshTokenResponse { Token = token, RefreshToken = refreshToken.Token });
            }
            catch (System.Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] AuctionHouse.Api.Models.RefreshTokenRequest model)
        {
            try
            {
                var refreshToken = await _refreshTokenService.ValidateAsync(model.RefreshToken);
                if (refreshToken == null)
                    return Unauthorized(new { message = "Invalid refresh token" });

                var user = await _authService.GetUserByIdAsync(refreshToken.UserId);               

                await _refreshTokenService.InvalidateAsync(model.RefreshToken);                
                var newRefreshToken = await _refreshTokenService.GenerateAsync(user.Id);
                var newToken = _jwtTokenGenerator.GenerateToken(user);
                
                return Ok(new AuctionHouse.Api.Models.RefreshTokenResponse { Token = newToken, RefreshToken = newRefreshToken.Token });
            }
            catch (System.Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
