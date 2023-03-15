using Domain.Models.Pagination;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Domain.IContracts.IRepositories.IGenericRepositories
{
    public interface IQueriesRepository<T> where T : class
    {
        IQueryable<T> Query();
        void Attach(T obj);
        List<T> BindList(DataTable dt);
        SqlParameter CreateSqlParameter(string ParameterName, object value);


        int ExecuteQueryScalar(string dmlQuery, params SqlParameter[] parameters);
        void ExecuteQueryNonScalar(string dmlQuery, bool IsStoredProcedure = false, params SqlParameter[] parameters);

        DataTable GetDataTableFromQuery(string sqlQuery, bool IsStoredProcedure = false, params SqlParameter[] parameters);
        DataSet GetDataSetFromQuery(string sqlQuery, bool IsStoredProcedure = false, params SqlParameter[] parameters);

        IQueryable<T> ExecuteSqlQuery(string sqlQuery, params SqlParameter[] parameters);

        List<TResponse> ExecuteSqlStoredProcedure<TResponse>(string sqlQuery, params SqlParameter[] parameters) where TResponse : class;
        List<T> ExecuteSqlStoredProcedure(string StoredProcedureName, params SqlParameter[] parameters);
        List<T> ExecuteSqlStoredProcedure(string StoredProcedureName, Pagination pagination, List<SqlParameter> parameters);
        Task<PaginatedList<TResponse>> ExecuteSqlStoredProcedureAsync<TResponse>(string StoredProcedureName, Pagination pagination, List<SqlParameter> parameters) where TResponse : class;
    }
}
