using AuctionHouse.Domain.Entities;

namespace AuctionHouse.Application.Interfaces;

public interface IAuctionService
{
    Task<Auction?> GetByIdAsync(Guid id);
    Task<IEnumerable<Auction>> GetAllAsync();
    Task<IEnumerable<Auction>> GetActiveAsync();
    Task<Auction> CreateAsync(string sellerId, string itemName, string description, decimal minimumPrice, DateTime endsAt);
    Task UpdateAsync(Guid id, string callerId, string itemName, string description, decimal minimumPrice, DateTime endsAt);
    Task PublishAsync(Guid id, string callerId);
    Task UnpublishAsync(Guid id, string callerId);
    Task CloseExpiredAsync();
    Task CancelAsync(Guid id, string callerId);
}
