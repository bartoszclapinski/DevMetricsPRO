namespace DevMetricsPro.Application.Interfaces;

/// <summary>
/// Simple abstraction over the distributed cache implementation.
/// Keeps caching logic inside the Infrastructure layer while allowing
/// Application/Web layers to depend on an interface.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Attempts to retrieve a cached value.
    /// </summary>
    /// <typeparam name="T">Type of the cached payload.</typeparam>
    /// <param name="key">Cache key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached value or null when missing.</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores a value in cache for the specified duration.
    /// </summary>
    /// <typeparam name="T">Type of the payload.</typeparam>
    /// <param name="key">Cache key.</param>
    /// <param name="value">Payload to cache.</param>
    /// <param name="ttl">Expiration duration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a cache entry if it exists.
    /// </summary>
    /// <param name="key">Cache key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}

