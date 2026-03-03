namespace AuctionHouse.Api.Models
{
    public class RefreshTokenRequest
    {
        public required string RefreshToken { get; set; }
    }

    public class RefreshTokenResponse
    {
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
    }
}
