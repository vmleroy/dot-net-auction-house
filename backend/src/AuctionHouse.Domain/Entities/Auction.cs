namespace AuctionHouse.Domain.Entities;

public class Auction
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string SellerId { get; set; } = string.Empty;
    public User Seller { get; set; } = null!;

    public string ItemName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal MinimumPrice { get; set; }
    public bool IsPublished { get; set; } = false;

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime EndsAt { get; set; }
    public bool IsClosed { get; set; } = false;
    public bool IsActive => IsPublished && !IsClosed && DateTime.UtcNow >= StartedAt && DateTime.UtcNow < EndsAt;

    public List<Bid> Bids { get; set; } = new List<Bid>();
}