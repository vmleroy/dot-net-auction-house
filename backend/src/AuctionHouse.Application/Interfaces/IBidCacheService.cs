using AuctionHouse.Domain.Entities;

namespace AuctionHouse.Application.Interfaces;

public interface IBidCacheService
{
    Task<Bid> GetHighestBidAsync(Guid auctionId);
    Task SetHighestBidAsync(Guid auctionId, Bid bid);
}
