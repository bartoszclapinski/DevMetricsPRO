using System.Linq.Expressions;
using DevMetricsPro.Core.Interfaces;
using DevMetricsPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DevMetricsPro.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation for data access operations
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    /// <summary>
    /// Constructor initializes the DbSet for the entity type
    /// </summary>
    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <summary>
    /// Get an entity by its ID
    /// </summary>
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    /// <summary>
    /// Get all entities (without tracking for read-only scenarios)
    /// </summary>
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Find entities matching a predicate
    /// </summary>
    public async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync(cancellationToken);    
    }

    /// <summary>
    /// Returns an IQueryable with AsNoTracking for composing read-only queries
    /// </summary>
    public IQueryable<T> Query()
    {
        return _dbSet.AsNoTracking();
    }

    /// <summary>
    /// Add a new entity
    /// </summary>
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <summary>
    /// Update an existing entity
    /// </summary>
    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Delete an existing entity
    /// </summary>
    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Count entities match optional predicate
    /// </summary>
    public async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null, 
        CancellationToken cancellationToken = default)
    {
        if (predicate == null) return await _dbSet.CountAsync(cancellationToken);
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }
    
}