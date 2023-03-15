#region Imports

using System.Linq.Expressions;

#endregion

namespace Domain.IContracts.IRepositories.IGenericRepositories;

public interface IGenericRepository<T> : IQueriesRepository<T> where T : class
{
    Task<T> GetAsync(int id);
    Task<T> GetByStringAsync(string id);

    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetWhereAndFromToAsync(int from, int to, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetWhereOrderByDescAsync<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderByDescSelector, params Expression<Func<T, object>>[] includes);

    Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<T> GetFirstOrDefaultOrderByDescAsync<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderByDescSelector);

    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entity);

    void Update(T entity);

    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);

    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

    IEnumerable<T> ExecWithStoreProcedure(string query, params object[] parameters);
}
