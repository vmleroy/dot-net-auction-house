using AuctionHouse.Application.Interfaces;
using StackExchange.Redis;
using Polly;
using Polly.Retry;

namespace AuctionHouse.Infrastructure.Services;

public class RedisLockService : ILockService
{
    private readonly IDatabase _database;
    private readonly AsyncRetryPolicy<bool> _retryPolicy;

    public RedisLockService(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
        _retryPolicy = Policy
            .Handle<TimeoutException>()
            .OrResult<bool>(acquired => acquired == false)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, retryAttempt - 1)));
    }

    public async Task<T> ExecuteWithLockAsync<T>(string key, TimeSpan expiration, Func<Task<T>> action)
    {
        var lockKey = $"lock:{key}";
        var lockValue = Guid.NewGuid().ToString();
        var acquired = await _retryPolicy.ExecuteAsync(() => 
            _database.LockTakeAsync(lockKey, lockValue, expiration));

        if (acquired)
        {
            try
            {
                return await action();
            }
            finally
            {
                await _database.LockReleaseAsync(lockKey, lockValue);
            }
        }

        throw new TimeoutException($"Could not acquire lock for key: {key} after multiple attempts.");
    }
}