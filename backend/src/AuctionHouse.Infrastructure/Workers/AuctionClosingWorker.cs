using AuctionHouse.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuctionHouse.Infrastructure.Workers;

public class AuctionClosingWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuctionClosingWorker> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

    public AuctionClosingWorker(IServiceScopeFactory scopeFactory, ILogger<AuctionClosingWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AuctionClosingWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var auctionService = scope.ServiceProvider.GetRequiredService<IAuctionService>();
                await auctionService.CloseExpiredAsync();
                _logger.LogInformation("Expired auctions processed at {Time}.", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while closing expired auctions.");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}
