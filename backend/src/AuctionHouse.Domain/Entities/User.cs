using Microsoft.AspNetCore.Identity;

namespace AuctionHouse.Domain.Entities;

public class User : IdentityUser
{
    public decimal Balance { get; set; }

    public List<Auction> UserAuctions { get; set; } = new List<Auction>();
    public List<Bid> UserBids { get; set; } = new List<Bid>();
}