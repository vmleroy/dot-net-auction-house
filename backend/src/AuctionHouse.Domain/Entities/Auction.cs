namespace AuctionHouse.Domain.Entities;

public class Auction
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string SellerId { get; set; } = string.Empty;
    public User Seller { get; set; } = null!;

    public string ItemName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal MinimumPrice { get; set; }

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime EndsAt { get; set; }
    public bool IsActive => DateTime.UtcNow >= StartedAt && DateTime.UtcNow < EndsAt;

    public List<Bid> Bids { get; set; } = new List<Bid>();

    public bool IsValidBid(decimal amount, decimal currentHighestBid) => 
        amount > currentHighestBid && amount > MinimumPrice && IsActive;

}