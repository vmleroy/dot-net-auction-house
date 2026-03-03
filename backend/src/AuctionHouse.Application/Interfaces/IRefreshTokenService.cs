using System.Threading.Tasks;
using AuctionHouse.Domain.Entities;

namespace AuctionHouse.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> GenerateAsync(string userId, bool persistent = false);
        Task<RefreshToken?> ValidateAsync(string token);
        Task InvalidateAsync(string token);
    }
}
