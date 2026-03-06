using AuctionHouse.Domain.Entities;

namespace AuctionHouse.Domain.Repositories;

public interface IAuctionRepository
{
    Task<Auction?> GetByIdAsync(Guid id);
    Task<IEnumerable<Auction>> GetAllAsync();
    Task<IEnumerable<Auction>> GetActiveAsync();
    Task<IEnumerable<Auction>> GetExpiredUnclosedAsync();
    Task AddAsync(Auction auction);
    Task UpdateAsync(Auction auction);
    Task CloseAsync(Guid id);
    Task DeleteAsync(Guid id);
}
