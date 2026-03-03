
using AuctionHouse.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouse.Infrastructure.Data;

public class AuctionDbContext : IdentityDbContext<User>
{
    public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options) { }

    public DbSet<Auction> Auctions { get; set; }
    public DbSet<Bid> Bids { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auction>()
            .Property(a => a.MinimumPrice)
            .HasPrecision(18, 2);

        base.OnModelCreating(modelBuilder);
    }
}