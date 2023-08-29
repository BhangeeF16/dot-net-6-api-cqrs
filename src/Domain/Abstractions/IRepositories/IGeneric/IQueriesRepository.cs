using Domain.Models.Pagination;
using Microsoft.Data.SqlClient;

namespace Domain.Abstractions.IRepositories.IGeneric;

public interface IQueriesRepository<T> where T : class
{
    int ExecuteQueryScalar(string dmlQuery, params SqlParameter[] parameters);
    void ExecuteQueryNonScalar(string dmlQuery, bool IsStoredProcedure = false, params SqlParameter[] parameters);

    IQueryable<T> ExecuteSqlQuery(string sqlQuery, params SqlParameter[] parameters);
    List<TResponse> ExecuteSqlQuery<TResponse>(string sqlQuery, params SqlParameter[] parameters) where TResponse : class;

    List<T> ExecuteSqlStoredProcedure(string StoredProcedureName, params SqlParameter[] parameters);
    List<TResponse> ExecuteSqlStoredProcedure<TResponse>(string sqlQuery, params SqlParameter[] parameters) where TResponse : class;
    Task<PaginatedList<TResponse>> ExecuteSqlStoredProcedureAsync<TResponse>(string StoredProcedureName, Pagination pagination, List<SqlParameter> parameters = null) where TResponse : class;
}
