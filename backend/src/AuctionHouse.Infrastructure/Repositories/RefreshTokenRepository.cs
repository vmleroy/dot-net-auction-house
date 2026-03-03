using System.Threading.Tasks;
using AuctionHouse.Domain.Entities;
using AuctionHouse.Domain.Repositories;
using AuctionHouse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouse.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuctionDbContext _context;
        public RefreshTokenRepository(AuctionDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> CreateAsync(RefreshToken token)
        {
            _context.Set<RefreshToken>().Add(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
            var refreshToken = await _context.Set<RefreshToken>().FirstOrDefaultAsync(t => t.Token == token);
            return refreshToken ?? throw new InvalidOperationException("Refresh token not found.");
        }

        public async Task InvalidateAsync(RefreshToken token)
        {
            token.Revoked = System.DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
