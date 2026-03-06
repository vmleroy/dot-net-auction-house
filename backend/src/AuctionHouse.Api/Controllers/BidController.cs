using AuctionHouse.Api.Models;
using AuctionHouse.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuctionHouse.Api.Controllers;

[ApiController]
[Route("api/auction/{auctionId:guid}/bid")]
public class BidController : ControllerBase
{
    private readonly IBidService _bidService;

    public BidController(IBidService bidService)
    {
        _bidService = bidService;
    }

    [HttpGet("highest")]
    public async Task<IActionResult> GetHighest(Guid auctionId)
    {
        try
        {
            var bid = await _bidService.GetHighestBid(auctionId);
            return Ok(bid);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> PlaceBid(Guid auctionId, [FromBody] PlaceBidRequest request)
    {
        try
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("User not authenticated.");

            var userId = Guid.Parse(userIdString);
            var bid = await _bidService.PlaceBid(auctionId, userId, request.Amount);
            return Ok(bid);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
