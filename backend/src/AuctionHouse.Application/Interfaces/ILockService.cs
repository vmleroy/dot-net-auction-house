namespace AuctionHouse.Application.Interfaces;

public interface ILockService
{
    Task<T> ExecuteWithLockAsync<T>(string key, TimeSpan expiration, Func<Task<T>> action);
}