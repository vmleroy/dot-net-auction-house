using StackExchange.Redis;

namespace AuctionHouse.Infrastructure.Services;

public class BidCacheService
{
    private readonly IDatabase _cache;

    public BidCacheService(IConnectionMultiplexer redis)
    {
        _cache = redis.GetDatabase();
    }

    public async Task SetHighestBid(Guid auctionId, decimal amount)
    {
        await _cache.StringSetAsync($"auction:{auctionId}:highest", amount.ToString());
    }

    public async Task<decimal> GetHighestBid(Guid auctionId)
    {
        var val = await _cache.StringGetAsync($"auction:{auctionId}:highest");
        return val.HasValue ? decimal.Parse(val!) : 0;
    }
}