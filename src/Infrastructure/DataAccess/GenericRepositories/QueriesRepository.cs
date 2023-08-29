using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Extensions;
using Domain.ConfigurationOptions;
using Domain.Models.Pagination;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace Infrastructure.DataAccess.GenericRepositories;

public class QueriesRepository<T> : IQueriesRepository<T> where T : class
{
    #region Constructors and Locals

    private readonly ApplicationDbContext _context;
    private readonly InfrastructureOptions _connectionInfo;
    public QueriesRepository(ApplicationDbContext context, InfrastructureOptions infrastructureOptions)
    {
        _context = context;
        _connectionInfo = infrastructureOptions;
        _context.Database.SetCommandTimeout(4800); // Set database request timeout 8 mintutes
    }

    #endregion

    #region Default Methods

    private List<T> BindList(DataTable dt)
    {
        var serializeString = JsonConvert.SerializeObject(dt);
        return JsonConvert.DeserializeObject<List<T>>(serializeString);
    }
    private static TResponse BindObject<TResponse>(DataTable dt) where TResponse : class
    {
        var serializeString = JsonConvert.SerializeObject(dt);
        return JsonConvert.DeserializeObject<List<TResponse>>(serializeString)?.FirstOrDefault();
    }
    private static PagingData BindPagingData(DataTable dt)
    {
        var serializeString = JsonConvert.SerializeObject(dt);
        return JsonConvert.DeserializeObject<List<PagingData>>(serializeString)?.FirstOrDefault() ?? new PagingData();
    }
    private static List<TResponse> BindList<TResponse>(DataTable dt) where TResponse : class
    {
        var serializeString = JsonConvert.SerializeObject(dt);
        return serializeString == "[]" ? new List<TResponse>() : JsonConvert.DeserializeObject<List<TResponse>>(serializeString);
    }

    #endregion

    #region QUERIES

    public List<T> ExecuteSqlStoredProcedure(string sqlQuery, params SqlParameter[] parameters)
    {
        var storedProcedureResult = GetDataTableFromQuery(sqlQuery, true, parameters);
        return BindList(storedProcedureResult);
    }
    public IQueryable<T> ExecuteSqlQuery(string sqlQuery, params SqlParameter[] sqlParameters)
    {
        if (sqlParameters != null && sqlParameters.Any())
        {
            return _context.Set<T>().FromSqlRaw(sqlQuery, sqlParameters).IgnoreQueryFilters().AsQueryable();
        }
        return _context.Set<T>().FromSqlRaw(sqlQuery).AsQueryable();
    }
    public List<TResponse> ExecuteSqlStoredProcedure<TResponse>(string sqlQuery, params SqlParameter[] parameters) where TResponse : class
    {
        var storedProcedureResult = GetDataTableFromQuery(sqlQuery, true, parameters);
        return BindList<TResponse>(storedProcedureResult);
    }
    public List<TResponse> ExecuteSqlQuery<TResponse>(string sqlQuery, params SqlParameter[] parameters) where TResponse : class
    {
        var storedProcedureResult = GetDataTableFromQuery(sqlQuery, false, parameters);
        return BindList<TResponse>(storedProcedureResult);
    }
    public async Task<PaginatedList<TResponse>> ExecuteSqlStoredProcedureAsync<TResponse>(string sqlQuery, Pagination pagination, List<SqlParameter> parameters = null) where TResponse : class
    {
        parameters = parameters != null && parameters.Any() ? parameters : new List<SqlParameter>();

        var totalCountOutput = new SqlParameter("@totalCount", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        parameters.Add(totalCountOutput);
        parameters.Add(new SqlParameter("@pageNumber", pagination.PageNumber == null ? DBNull.Value : pagination.PageNumber));
        parameters.Add(new SqlParameter("@pageSize", pagination.PageSize == null ? DBNull.Value : pagination.PageSize));
        parameters.Add(new SqlParameter("@sortingCol", pagination.SortingCol == null ? DBNull.Value : pagination.SortingCol));
        parameters.Add(new SqlParameter("@sortDirection", pagination.SortDirection == null ? DBNull.Value : pagination.SortDirection));
        parameters.Add(new SqlParameter("@keyword", pagination.Keyword == null ? DBNull.Value : pagination.Keyword));

        var storedProcedureResult = GetDataTableFromQuery(sqlQuery, true, parameters.ToArray());
        var mappedData = BindList<TResponse>(storedProcedureResult);
        var totalCount = Convert.ToInt32(totalCountOutput.Value == DBNull.Value ? 0 : totalCountOutput.Value);
        return await mappedData.PaginatedListAsync(pagination.PageNumber ?? 1, pagination.PageSize ?? 10, totalCount, pagination.Keyword ?? string.Empty);
    }

    #endregion

    #region ADO .NET

    public int ExecuteQueryScalar(string dmlQuery, params SqlParameter[] parameters)
    {
        var ID = 0;
        using (var connection = new SqlConnection(_connectionInfo.ConnectionString))
        {
            using (var cmd = new SqlCommand(dmlQuery, connection))
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(parameters);
                ID = (int)cmd.ExecuteScalar();
            }
        }
        return ID;
    }
    public void ExecuteQueryNonScalar(string dmlQuery, bool IsStoredProcedure = false, params SqlParameter[] parameters)
    {
        using (var connection = new SqlConnection(_connectionInfo.ConnectionString))
        {
            connection.Open();
            using (var cmd = new SqlCommand(dmlQuery, connection))
            {
                if (IsStoredProcedure)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                }
                cmd.Parameters.Clear();
                if (parameters != null && parameters.Any())
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }
    }


    private DataSet GetDataSetFromQuery(string sqlQuery, bool IsStoredProcedure = false, params SqlParameter[] parameters)
    {
        var ds = new DataSet();
        using (var connection = new SqlConnection(_connectionInfo.ConnectionString))
        {
            connection.Open();
            var da = new SqlDataAdapter(sqlQuery, connection);
            da.SelectCommand.Parameters.Clear();
            da.SelectCommand.Parameters.AddRange(parameters);
            da.SelectCommand.CommandTimeout = 4800;
            da.SelectCommand.CommandType = IsStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            da.Fill(ds);
        }
        return ds;
    }
    private DataTable GetDataTableFromQuery(string sqlQuery, bool IsStoredProcedure = false, params SqlParameter[] parameters)
    {
        var dt = new DataTable();
        using (var connection = new SqlConnection(_connectionInfo.ConnectionString))
        {
            connection.Open();
            var da = new SqlDataAdapter(sqlQuery, connection);
            da.SelectCommand.Parameters.Clear();
            if (parameters != null && parameters.Any())
            {
                da.SelectCommand.Parameters.AddRange(parameters);
            }
            da.SelectCommand.CommandTimeout = 4800;
            da.SelectCommand.CommandType = IsStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            da.Fill(dt);
        }
        return dt;
    }
    private DataTable GetDataTableFromQuery(string sqlQuery, bool IsStoredProcedure = false)
    {
        var dt = new DataTable();
        using (var connection = new SqlConnection(_connectionInfo.ConnectionString))
        {
            connection.Open();
            var da = new SqlDataAdapter(sqlQuery, connection);
            da.SelectCommand.Parameters.Clear();
            da.SelectCommand.CommandTimeout = 4800;
            da.SelectCommand.CommandType = IsStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            da.Fill(dt);
        }
        return dt;
    }
    private SqlParameter CreateSqlParameter(string ParameterName, object value) => new SqlParameter(ParameterName, value);

    #endregion
}
