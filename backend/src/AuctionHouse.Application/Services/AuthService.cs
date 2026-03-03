using System.Threading.Tasks;
using AuctionHouse.Application.Interfaces;
using AuctionHouse.Domain.Repositories;

namespace AuctionHouse.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly Microsoft.AspNetCore.Identity.IPasswordHasher<Domain.Entities.User> _passwordHasher;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Domain.Entities.User>();
        }

        public async Task<Domain.Entities.User> RegisterUserAsync(string username, string email, string password)
        {
            var existingUserByEmail = await _userRepository.GetUserByEmailAsync(email);
            if (existingUserByEmail != null)
                throw new InvalidOperationException($"Email '{email}' is already taken.");

            var existingUserByUsername = await _userRepository.GetUserByUsernameAsync(username);
            if (existingUserByUsername != null)
                throw new InvalidOperationException($"Username '{username}' is already taken.");

            var user = new Domain.Entities.User { UserName = username, Email = email };
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            var createdUser = await _userRepository.CreateUserAsync(user.UserName, user.Email, user.PasswordHash);
            return createdUser ?? throw new InvalidOperationException("User registration failed.");
        }

        public async Task<Domain.Entities.User> ValidateUserAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                throw new InvalidOperationException("Invalid email or password.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
                throw new InvalidOperationException("Invalid email or password.");

            return user;
        }

        public async Task<Domain.Entities.User> GetUserByIdAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            return user ?? throw new InvalidOperationException("User not found.");
        }
    }
}
