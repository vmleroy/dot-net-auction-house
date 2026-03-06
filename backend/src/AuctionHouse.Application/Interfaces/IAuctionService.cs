using AuctionHouse.Domain.Entities;

namespace AuctionHouse.Application.Interfaces;

public interface IAuctionService
{
    Task<Auction?> GetByIdAsync(Guid id);
    Task<IEnumerable<Auction>> GetAllAsync();
    Task<IEnumerable<Auction>> GetActiveAsync();
    Task<Auction> CreateAsync(string sellerId, string itemName, string description, decimal minimumPrice, DateTime endsAt);
    Task UpdateAsync(Guid id, string itemName, string description, decimal minimumPrice, DateTime endsAt);
    Task PublishAsync(Guid id);
    Task UnpublishAsync(Guid id);
    Task CloseExpiredAsync();
    Task DeleteAsync(Guid id);
}
