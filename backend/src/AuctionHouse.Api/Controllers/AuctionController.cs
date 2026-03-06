using AuctionHouse.Api.Models;
using AuctionHouse.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuctionHouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionController : ControllerBase
{
    private readonly IAuctionService _auctionService;

    public AuctionController(IAuctionService auctionService)
    {
        _auctionService = auctionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var auctions = await _auctionService.GetAllAsync();
        return Ok(auctions);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var auctions = await _auctionService.GetActiveAsync();
        return Ok(auctions);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var auction = await _auctionService.GetByIdAsync(id);
        if (auction is null) return NotFound();
        return Ok(auction);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAuctionRequest request)
    {
        try
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("User not authenticated.");

            var auction = await _auctionService.CreateAsync(
                sellerId,
                request.ItemName,
                request.Description,
                request.MinimumPrice,
                request.EndsAt);

            return CreatedAtAction(nameof(GetById), new { id = auction.Id }, auction);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAuctionRequest request)
    {
        try
        {
            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("User not authenticated.");

            await _auctionService.UpdateAsync(
                id,
                callerId,
                request.ItemName,
                request.Description,
                request.MinimumPrice,
                request.EndsAt);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPatch("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    {
        try
        {
            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("User not authenticated.");

            await _auctionService.PublishAsync(id, callerId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPatch("{id:guid}/unpublish")]
    public async Task<IActionResult> Unpublish(Guid id)
    {
        try
        {
            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("User not authenticated.");

            await _auctionService.UnpublishAsync(id, callerId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("User not authenticated.");

            await _auctionService.CancelAsync(id, callerId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
