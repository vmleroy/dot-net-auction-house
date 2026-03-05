using AuctionHouse.Domain.Entities;
using AuctionHouse.Domain.Repositories;
using AuctionHouse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouse.Infrastructure.Repositories
{
    public class BidRepository : IBidRepository
    {
        private readonly AuctionDbContext _context;

        public BidRepository(AuctionDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Bid bid)
        {
            await _context.Bids.AddAsync(bid);
            await _context.SaveChangesAsync();
        }

        public async Task<Bid> GetMaxBidForAuction(Guid auctionId)
        {
            var maxBid = await _context.Bids
                .Where(b => b.AuctionId == auctionId)
                .OrderByDescending(b => b.Amount)
                .FirstOrDefaultAsync();

            return maxBid ?? new Bid(auctionId, Guid.Empty.ToString(), 0);
        }
    }
}