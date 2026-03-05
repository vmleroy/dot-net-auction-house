using AuctionHouse.Application.Interfaces;
using AuctionHouse.Domain.Entities;
using AuctionHouse.Domain.Repositories;

namespace AuctionHouse.Application.Services;

public class BidService : IBidService
{
    private readonly IBidRepository _repository;
    private readonly ILockService _lockService;
    private readonly IBidCacheService _cacheService;

    public BidService(IBidRepository repository, ILockService lockService, IBidCacheService cacheService)
    {
        _repository = repository;
        _lockService = lockService;
        _cacheService = cacheService;
    }

    public async Task<Bid> GetHighestBid(Guid auctionId)
    {
        return await _cacheService.GetHighestBidAsync(auctionId);
    }

    public async Task<Bid> PlaceBid(Guid auctionId, Guid userId, decimal amount)
    {
        return await _lockService.ExecuteWithLockAsync($"auction:{auctionId}", TimeSpan.FromSeconds(5), async () =>
        {
            var currentHighest = await GetHighestBid(auctionId);
            if (amount <= currentHighest.Amount)
                throw new InvalidOperationException("Bid amount must be higher than the current highest bid.");

            var bid = new Bid(auctionId, userId.ToString(), amount);
            await _repository.AddAsync(bid);
            await _cacheService.SetHighestBidAsync(auctionId, bid);

            return bid;
        });
    }
}
