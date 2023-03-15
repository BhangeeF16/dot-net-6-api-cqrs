#region Imports

using Domain.ConfigurationOptions;
using Domain.IContracts.IRepositories.IGenericRepositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

#endregion

namespace Infrastructure.DataAccess.GenericRepositories;

public class GenericRepository<T> : QueriesRepository<T>, IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext _dbContext;
    protected readonly InfrastructureOptions _connectionInfo;
    public GenericRepository(ApplicationDbContext context, InfrastructureOptions connectionInfo) : base(context, connectionInfo)
    {
        _dbContext = context;
        _connectionInfo = connectionInfo;
    }
    public async Task<T> GetAsync(int id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }
    public async Task<T> GetByStringAsync(string id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }
    public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        var query = _dbContext.Set<T>().AsQueryable();
        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.ToListAsync();
    }
    public IQueryable<T> GetAllQueryable()
    {
        return _dbContext.Set<T>().AsQueryable();
    }
    public async Task AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
    }
    public void Add(T entity)
    {
        _dbContext.Set<T>().Add(entity);
    }
    public async Task AddRangeAsync(IEnumerable<T> entity)
    {
        await _dbContext.Set<T>().AddRangeAsync(entity);
    }
    public async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        var query = _dbContext.Set<T>().AsQueryable();
        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.Where(predicate).ToListAsync();
    }
    public async Task<IEnumerable<T>> GetWhereAndFromToAsync(int from, int to, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        var query = _dbContext.Set<T>().AsQueryable();
        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        // to is acting as count
        return await query.Where(predicate).Skip(from).Take(to).ToListAsync();
    }
    public async Task<IEnumerable<T>> GetWhereOrderByDescAsync<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderByDescSelector, params Expression<Func<T, object>>[] includes)
    {
        var query = _dbContext.Set<T>().AsQueryable();
        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.Where(predicate).OrderByDescending(orderByDescSelector).ToListAsync();
    }
    public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
    }
    public async Task<T> GetFirstOrDefaultOrderByDescAsync<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderByDescSelector)
    {
        return await _dbContext.Set<T>().OrderByDescending(orderByDescSelector).FirstOrDefaultAsync(predicate);
    }
    public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        var query = _dbContext.Set<T>().AsQueryable();
        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.FirstOrDefaultAsync(predicate) ?? default;
    }
    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbContext.Set<T>().AnyAsync(predicate);
    }
    public void Delete(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
    }
    public void DeleteRange(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().RemoveRange(entities);
    }
    public void Update(T entity)
    {
        _dbContext.Set<T>().Update(entity);
    }
    public IEnumerable<T> ExecWithStoreProcedure(string query, params object[] parameters)
    {
        return _dbContext.Set<T>().FromSqlRaw(query, parameters).ToList();
    }
}