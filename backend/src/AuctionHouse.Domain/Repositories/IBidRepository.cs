using AuctionHouse.Domain.Entities;

namespace AuctionHouse.Domain.Repositories
{
    public interface IBidRepository
    {
        Task AddAsync(Bid bid);
        Task<Bid> GetMaxBidForAuction(Guid auctionId);
    }
}