namespace DevMetricsPro.Core.Interfaces;

/// <summary>
/// Unit of Work pattern for managing transactions and coordinating repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Get a repository for specific entity type
    /// </summary>    
    IRepository<T> Repository<T>() where T : class;

    /// <summary>
    /// Save all changes to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begin a database transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit the current transaction
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollback the current transaction
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}