using AuctionHouse.Domain.Entities;
namespace AuctionHouse.Application.Interfaces
{
    public interface IAuthService
    {
        Task<User> RegisterUserAsync(string username, string email, string password);
        Task<User> ValidateUserAsync(string email, string password);
        Task<User> GetUserByIdAsync(string userId);
    }
}
