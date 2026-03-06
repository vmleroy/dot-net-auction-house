using AuctionHouse.Domain.Entities;
using AuctionHouse.Domain.Repositories;
using AuctionHouse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouse.Infrastructure.Repositories;

public class AuctionRepository : IAuctionRepository
{
    private readonly AuctionDbContext _context;

    public AuctionRepository(AuctionDbContext context)
    {
        _context = context;
    }

    public async Task<Auction?> GetByIdAsync(Guid id)
    {
        return await _context.Auctions
            .Include(a => a.Bids)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Auction>> GetAllAsync()
    {
        return await _context.Auctions
            .Include(a => a.Bids)
            .ToListAsync();
    }

    public async Task<IEnumerable<Auction>> GetActiveAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Auctions
            .Include(a => a.Bids)
            .Where(a => a.StartedAt <= now && a.EndsAt > now)
            .ToListAsync();
    }

    public async Task<IEnumerable<Auction>> GetExpiredUnclosedAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Auctions
            .Include(a => a.Bids)
            .Where(a => !a.IsClosed && a.EndsAt <= now)
            .ToListAsync();
    }

    public async Task AddAsync(Auction auction)
    {
        await _context.Auctions.AddAsync(auction);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Auction auction)
    {
        _context.Auctions.Update(auction);
        await _context.SaveChangesAsync();
    }

    public async Task CloseAsync(Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);
        if (auction is not null)
        {
            auction.IsClosed = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);
        if (auction is not null)
        {
            _context.Auctions.Remove(auction);
            await _context.SaveChangesAsync();
        }
    }
}
