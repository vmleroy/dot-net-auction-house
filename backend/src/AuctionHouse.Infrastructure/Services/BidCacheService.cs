using System.Text.Json;
using AuctionHouse.Application.Interfaces;
using AuctionHouse.Domain.Entities;
using AuctionHouse.Domain.Repositories;
using StackExchange.Redis;

namespace AuctionHouse.Infrastructure.Services;

public class BidCacheService : IBidCacheService
{
    private readonly IDatabase _cache;
    private readonly IBidRepository _repository;

    public BidCacheService(IConnectionMultiplexer redis, IBidRepository repository)
    {
        _cache = redis.GetDatabase();
        _repository = repository;
    }

    public async Task<Bid> GetHighestBidAsync(Guid auctionId)
    {
        var val = await _cache.StringGetAsync($"auction:{auctionId}:highest");
        if (val.HasValue)
        {
            var cachedBid = JsonSerializer.Deserialize<Bid>(val.ToString());
            if (cachedBid != null) return cachedBid;
        }

        var highestFromDb = await _repository.GetMaxBidForAuction(auctionId);
        await SetHighestBidAsync(auctionId, highestFromDb);
        return highestFromDb;
    }

    public async Task SetHighestBidAsync(Guid auctionId, Bid bid)
    {
        var json = JsonSerializer.Serialize(bid);
        await _cache.StringSetAsync($"auction:{auctionId}:highest", json, TimeSpan.FromHours(1));
    }
}
