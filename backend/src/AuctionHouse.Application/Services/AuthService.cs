using System.Threading.Tasks;
using AuctionHouse.Application.Services;
using AuctionHouse.Domain.Repositories;

namespace AuctionHouse.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly Microsoft.AspNetCore.Identity.IPasswordHasher<Domain.Entities.User> _passwordHasher;

        public AuthService(IUserRepository userRepository, JwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Domain.Entities.User>();
        }


        public async Task<string> RegisterAsync(string username, string email, string password)
        {
            var existingUserByEmail = await _userRepository.GetUserByEmailAsync(email);
            if (existingUserByEmail != null) throw new InvalidOperationException($"Email '{email}' is already taken.");

            var existingUserByUsername = await _userRepository.GetUserByUsernameAsync(username);
            if (existingUserByUsername != null) throw new InvalidOperationException($"Username '{username}' is already taken.");

            var user = new Domain.Entities.User { UserName = username, Email = email };
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            await _userRepository.CreateUserAsync(user.UserName, user.Email, user.PasswordHash);
            
            var token = _jwtTokenGenerator.GenerateToken(user);
            return token;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email) ?? throw new InvalidOperationException("Invalid email or password.");
            if (string.IsNullOrEmpty(user.PasswordHash))
                throw new InvalidOperationException("Invalid email or password.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
                throw new InvalidOperationException("Invalid email or password.");
                
            var token = _jwtTokenGenerator.GenerateToken(user);
            return token;
        }
    }
}
