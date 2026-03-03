using System.Threading.Tasks;
using AuctionHouse.Domain.Entities;

namespace AuctionHouse.Domain.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> CreateAsync(RefreshToken token);
        Task<RefreshToken> GetByTokenAsync(string token);
        Task InvalidateAsync(RefreshToken token);
    }
}
