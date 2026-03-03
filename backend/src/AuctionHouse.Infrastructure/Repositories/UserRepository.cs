using System.Threading.Tasks;
using AuctionHouse.Domain.Entities;
using AuctionHouse.Domain.Repositories;
using AuctionHouse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouse.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuctionDbContext _context;
        public UserRepository(AuctionDbContext context)
        {
            _context = context;
        }


        public async Task<User> CreateUserAsync(string username, string email, string passwordHash)
        {
            var user = new User { UserName = username, Email = email, PasswordHash = passwordHash };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            return user;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }
    }
}
