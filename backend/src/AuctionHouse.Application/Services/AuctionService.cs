using AuctionHouse.Application.Interfaces;
using AuctionHouse.Domain.Entities;
using AuctionHouse.Domain.Repositories;

namespace AuctionHouse.Application.Services;

public class AuctionService : IAuctionService
{
    private readonly IAuctionRepository _repository;

    public AuctionService(IAuctionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Auction?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Auction>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<Auction>> GetActiveAsync()
    {
        return await _repository.GetActiveAsync();
    }

    public async Task<Auction> CreateAsync(string sellerId, string itemName, string description, decimal minimumPrice, DateTime endsAt)
    {
        if (endsAt <= DateTime.UtcNow)
            throw new ArgumentException("Auction end date must be in the future.");

        if (minimumPrice <= 0)
            throw new ArgumentException("Minimum price must be greater than zero.");

        var auction = new Auction
        {
            SellerId = sellerId,
            ItemName = itemName,
            Description = description,
            MinimumPrice = minimumPrice,
            EndsAt = endsAt
        };

        await _repository.AddAsync(auction);
        return auction;
    }

    public async Task UpdateAsync(Guid id, string callerId, string itemName, string description, decimal minimumPrice, DateTime endsAt)
    {
        var auction = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Auction {id} not found.");

        if (auction.SellerId != callerId)
            throw new UnauthorizedAccessException("Only the auction owner can update it.");

        if (auction.IsActive && auction.Bids.Count > 0)
            throw new InvalidOperationException("Cannot update an auction that already has bids.");

        auction.ItemName = itemName;
        auction.Description = description;
        auction.MinimumPrice = minimumPrice;
        auction.EndsAt = endsAt;

        await _repository.UpdateAsync(auction);
    }

    public async Task PublishAsync(Guid id, string callerId)
    {
        var auction = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Auction {id} not found.");

        if (auction.SellerId != callerId)
            throw new UnauthorizedAccessException("Only the auction owner can publish it.");

        auction.IsPublished = true;
        await _repository.UpdateAsync(auction);
    }

    public async Task UnpublishAsync(Guid id, string callerId)
    {
        var auction = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Auction {id} not found.");

        if (auction.SellerId != callerId)
            throw new UnauthorizedAccessException("Only the auction owner can unpublish it.");

        if (auction.IsActive && auction.Bids.Count > 0)
            throw new InvalidOperationException("Cannot unpublish an auction that already has bids.");

        auction.IsPublished = false;
        await _repository.UpdateAsync(auction);
    }

    public async Task CloseExpiredAsync()
    {
        var expired = await _repository.GetExpiredUnclosedAsync();
        foreach (var auction in expired)
            await _repository.CloseAsync(auction.Id);
    }

    public async Task CancelAsync(Guid id, string callerId)
    {
        var auction = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Auction {id} not found.");

        if (auction.SellerId != callerId)
            throw new UnauthorizedAccessException("Only the auction owner can cancel it.");

        if (auction.IsCancelled)
            throw new InvalidOperationException("Auction is already cancelled.");

        await _repository.CancelAsync(id);
    }
}
