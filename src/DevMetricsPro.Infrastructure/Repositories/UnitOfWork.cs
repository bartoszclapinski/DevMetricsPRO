using DevMetricsPro.Core.Interfaces;
using DevMetricsPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace DevMetricsPro.Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation for managing transactions and coordinating repositories
/// </summary>
public class UnitOfWork : IUnitOfWork
{    
    private readonly ApplicationDbContext _context;
    private readonly Dictionary<Type, object> _repositories;
    private IDbContextTransaction? _transaction;

    /// <summary>
    /// Constructor initializes the UnitOfWork with the DbContext
    /// </summary>
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        _repositories = new Dictionary<Type, object>();
    }

    /// <summary>
    /// Begin a database transaction
    /// </summary>    
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Commit current transaction
    /// </summary>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null) throw new InvalidOperationException("No active transaction to commit");
        try 
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);            
        } 
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Dispose the UnitOfWork and clean up resources
    /// </summary>
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Get a repository for a specific entity type
    /// Creates the repository if it doesn't exist yet (lazy loading)
    /// </summary>    
    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);

        // Check if we already have a repository for this type
        if (_repositories.ContainsKey(type)) return (IRepository<T>)_repositories[type];

        // Create a new repository and cache it
        var repository = new Repository<T>(_context);
        _repositories.Add(type, repository);
        return repository;
    }

    /// <summary>
    /// Rollback the current transaction
    /// </summary>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null) throw new InvalidOperationException("No active transaction to rollback");

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Save all changes to the database
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    
}
