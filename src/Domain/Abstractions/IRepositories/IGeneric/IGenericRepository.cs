#region Imports

using Domain.Models.Pagination;
using System.Linq.Expressions;

#endregion

namespace Domain.Abstractions.IRepositories.IGeneric;

public interface IGenericRepository<T> : IQueriesRepository<T> where T : class
{
    void Attach(T entity);
    IQueryable<T> Query();
    IQueryable<T> TableNoTracking { get; }

    Task<T> GetAsync(int id);
    Task<T> GetAsync(string id);

    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

    Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetWhereAsync(int index, int count, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetWhereAsync<TKey>(Expression<Func<T, bool>> predicate, string order = "DESC", Expression<Func<T, TKey>> selector = null, params Expression<Func<T, object>>[] includes);

    Task<PaginatedList<T>> GetPaginatedListAsync(Pagination pagination, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<T> GetFirstOrDefaultAsync<TKey>(Expression<Func<T, bool>> predicate, string order = "DESC", Expression<Func<T, TKey>> selector = null, params Expression<Func<T, object>>[] includes);

    void Add(T entity);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entity);

    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    Task UpdateWhereAsync(Expression<Func<T, bool>> predicate, Expression<Func<Microsoft.EntityFrameworkCore.Query.SetPropertyCalls<T>, Microsoft.EntityFrameworkCore.Query.SetPropertyCalls<T>>> setPropertyCalls);

    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);
    Task DeleteWhereAsync(Expression<Func<T, bool>> predicate);

    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    IEnumerable<T> ExecWithStoreProcedure(string query, params object[] parameters);
    Task<IEnumerable<T>> GetWhereNoTrackingAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<T> GetFirstOrDefaultNoTrackingAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<T> GetFirstOrDefaultNoTrackingAsync<TKey>(Expression<Func<T, bool>> predicate, string order = "DESC", Expression<Func<T, TKey>> selector = null, params Expression<Func<T, object>>[] includes);
    void DeAttach(T Entry);
}
