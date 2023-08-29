#region Imports

using Domain.Abstractions.IRepositories.IGeneric;
using Domain.ConfigurationOptions;
using Domain.Models.Pagination;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

#endregion

namespace Infrastructure.DataAccess.GenericRepositories;

public class GenericRepository<T> : QueriesRepository<T>, IGenericRepository<T> where T : class
{
    #region CONSTRUCTORS AND LOCALS

    protected readonly ApplicationDbContext _context;
    protected readonly InfrastructureOptions _infrastructureOptions;
    public GenericRepository(ApplicationDbContext context, InfrastructureOptions infrastructureOptions) : base(context, infrastructureOptions) => (_context, _infrastructureOptions) = (context, infrastructureOptions);    
    
    #endregion

    private DbSet<T>? _entities;
    protected virtual DbSet<T> Entities => _entities ??= _context.Set<T>();
    public IQueryable<T> TableNoTracking => Entities.AsNoTracking();
    public IQueryable<T> Query() => _context.Set<T>();

    public void Attach(T entity) => _context.Set<T>().Attach(entity);
    public void DeAttach(T Entry) => _context.Entry(Entry).State = EntityState.Detached;

    public async Task<T> GetAsync(int id) => await _context.Set<T>().FindAsync(id);
    public async Task<T> GetAsync(string id) => await _context.Set<T>().FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        var query = _context.Set<T>().AsQueryable();
        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<T>> GetWhereNoTrackingAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        var query = _context.Set<T>().AsQueryable();
        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.Where(predicate).AsNoTracking().ToListAsync();
    }
    public async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        var query = _context.Set<T>().AsQueryable();
        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.Where(predicate).ToListAsync();
    }
    public async Task<IEnumerable<T>> GetWhereAsync(int index, int count, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        var query = _context.Set<T>().AsQueryable();
        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        query = query.Where(predicate);

        query = query.Skip(index).Take(count);

        return await query.ToListAsync();
    }
    public async Task<IEnumerable<T>> GetWhereAsync<TKey>(Expression<Func<T, bool>> predicate, string order = "DESC", Expression<Func<T, TKey>> selector = null, params Expression<Func<T, object>>[] includes)
    {
        var query = _context.Set<T>().AsQueryable();
        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        query = order.Equals("DESC") ? query.OrderByDescending(selector) : query.OrderBy(selector);

        return await query.Where(predicate).ToListAsync();
    }
    public async Task<PaginatedList<T>> GetPaginatedListAsync(Pagination pagination, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        var pageNumber = pagination.PageNumber ?? 1;
        var pageSize = pagination.PageSize ?? 10;

        var query = _context.Set<T>().AsQueryable();

        query = query.Where(predicate);

        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        //if (!string.IsNullOrWhiteSpace(pagination.Keyword))
        //{
        //    var propertyList = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
        //    query = query.Where(item => propertyList.Any(property => property.Value != null && property.Value.ToString() == pagination.Keyword));
        //}

        if (!string.IsNullOrWhiteSpace(pagination.SortingCol))
        {
            var property = typeof(T).GetProperty(pagination.SortingCol);
            if (property != null)
            {

                // Create a parameter expression for the input object (e.g. p => ...)
                // Create a member expression that gets the value of the specified property (e.g. p.PropertyName)
                // Create a lambda expression that takes an input object and returns the value of the specified property (e.g. p => p.PropertyName)
                var parameter = Expression.Parameter(typeof(T), "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);

                Type propertyType = property.PropertyType;
                Type funcResultType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

                if (funcResultType == typeof(int) || funcResultType == typeof(int?))
                {
                    var orderByExpression = Expression.Lambda<Func<T, int>>(propertyAccess, parameter);
                    query = pagination.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(orderByExpression) : query.OrderBy(orderByExpression);
                }
                else if (funcResultType == typeof(string))
                {
                    var orderByExpression = Expression.Lambda<Func<T, string>>(propertyAccess, parameter);
                    query = pagination.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(orderByExpression) : query.OrderBy(orderByExpression);
                }
                else if (funcResultType == typeof(DateTime))
                {
                    var orderByExpression = Expression.Lambda<Func<T, DateTime>>(propertyAccess, parameter);
                    query = pagination.SortDirection?.ToLower() == "desc" ? query.OrderByDescending(orderByExpression) : query.OrderBy(orderByExpression);
                }
                else
                {
                    // add others if needed
                }
            }
        }

        IEnumerable<T> items = await query.ToListAsync();

        return await PaginatedList<T>.CreateAsync(items, pageNumber, pageSize);
    }

    public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate) => await _context.Set<T>().FirstOrDefaultAsync(predicate);
    public async Task<T> GetFirstOrDefaultNoTrackingAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        var query = _context.Set<T>().AsQueryable();
        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.AsNoTracking().FirstOrDefaultAsync(predicate) ?? default;
    }
    public async Task<T> GetFirstOrDefaultNoTrackingAsync<TKey>(Expression<Func<T, bool>> predicate, string order = "DESC", Expression<Func<T, TKey>> selector = null, params Expression<Func<T, object>>[] includes)
    {
        if (!order.Equals("DESC") && !order.Equals("ASC"))
        {
            throw new InvalidOperationException("invalid sort order provided");
        }

        var query = _context.Set<T>().AsQueryable();
        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        query = order.Equals("DESC") ? query.OrderByDescending(selector) : query.OrderBy(selector);

        return await query.AsNoTracking().FirstOrDefaultAsync(predicate) ?? default;
    }
    public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        var query = _context.Set<T>().AsQueryable();
        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.FirstOrDefaultAsync(predicate) ?? default;
    }
    public async Task<T> GetFirstOrDefaultAsync<TKey>(Expression<Func<T, bool>> predicate, string order = "DESC", Expression<Func<T, TKey>> selector = null, params Expression<Func<T, object>>[] includes)
    {
        if (!order.Equals("DESC") && !order.Equals("ASC"))
        {
            throw new InvalidOperationException("invalid sort order provided");
        }

        var query = _context.Set<T>().AsQueryable();
        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        query = order.Equals("DESC") ? query.OrderByDescending(selector) : query.OrderBy(selector);

        return await query.FirstOrDefaultAsync(predicate) ?? default;
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) => await _context.Set<T>().AnyAsync(predicate);

    public void Add(T entity) => _context.Set<T>().Add(entity);
    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        _context.Entry(entity).State = EntityState.Added;
    }
    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        if (entities != null && entities.Any())
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
        _context.Entry(entity).State = EntityState.Deleted;
    }
    public void DeleteRange(IEnumerable<T> entities)
    {
        if (entities != null && entities.Any())
        {
            _context.Set<T>().RemoveRange(entities);
        }
    }
    public async Task DeleteWhereAsync(Expression<Func<T, bool>> predicate) => await _context.Set<T>().Where(predicate).ExecuteDeleteAsync();

    public async Task UpdateWhereAsync(Expression<Func<T, bool>> predicate, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyCalls) => await _context.Set<T>().Where(predicate).ExecuteUpdateAsync(setPropertyCalls);
    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }
    public void UpdateRange(IEnumerable<T> entities)
    {
        if (entities != null && entities.Any())
        {
            _context.Set<T>().UpdateRange(entities);
        }
    }
    public IEnumerable<T> ExecWithStoreProcedure(string query, params object[] parameters) => _context.Set<T>().FromSqlRaw(query, parameters).ToList();
}
