namespace AuctionHouse.Api.Models;

public class CreateAuctionRequest
{
    public required string ItemName { get; set; }
    public required string Description { get; set; }
    public decimal MinimumPrice { get; set; }
    public DateTime EndsAt { get; set; }
}

public class UpdateAuctionRequest
{
    public required string ItemName { get; set; }
    public required string Description { get; set; }
    public decimal MinimumPrice { get; set; }
    public DateTime EndsAt { get; set; }
}
