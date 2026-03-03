using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AuctionHouse.Domain.Entities;
using AuctionHouse.Domain.Repositories;

using AuctionHouse.Application.Interfaces;
namespace AuctionHouse.Application.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        private static string GenerateSecureToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<RefreshToken> GenerateAsync(string userId, bool persistent = false)
        {
            var token = new RefreshToken
            {
                Token = GenerateSecureToken(),
                UserId = userId,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(persistent ? 30 : 7)
            };
            await _refreshTokenRepository.CreateAsync(token);
            return token;
        }

        public async Task<RefreshToken?> ValidateAsync(string token)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token);
            if (refreshToken == null || !refreshToken.IsActive)
                return null;
            return refreshToken;
        }

        public async Task InvalidateAsync(string token)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token);
            if (refreshToken != null)
                await _refreshTokenRepository.InvalidateAsync(refreshToken);
        }
    }
}
