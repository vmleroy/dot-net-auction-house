using System.Threading.Tasks;
using AuctionHouse.Domain.Entities;

namespace AuctionHouse.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(string username, string email, string password);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(string userId);
    }
}
