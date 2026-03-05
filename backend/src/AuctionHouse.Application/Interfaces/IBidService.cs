using AuctionHouse.Domain.Entities;

namespace AuctionHouse.Application.Interfaces
{
    public interface IBidService
    {
        Task<Bid> PlaceBid(Guid auctionId, Guid userId, decimal amount);
        Task<Bid> GetHighestBid(Guid auctionId);
    }
}
