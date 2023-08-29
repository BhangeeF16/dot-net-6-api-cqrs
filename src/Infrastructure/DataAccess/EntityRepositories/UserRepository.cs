using Domain.Abstractions.IRepositories.IEntity;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Constants;
using Domain.ConfigurationOptions;
using Domain.Entities.UsersModule;
using Domain.Models.Pagination;
using Infrastructure.DataAccess.GenericRepositories;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;

namespace Infrastructure.DataAccess.EntityRepositories;
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context, InfrastructureOptions infrastructureOptions) : base(context, infrastructureOptions) { }

    #region Inner-Repositories

    private IGenericRepository<UserLoginHistory>? _userLoginHistories;
    public IGenericRepository<UserLoginHistory> LoginHistories => _userLoginHistories ??= new GenericRepository<UserLoginHistory>(_context, _infrastructureOptions);

    #endregion

    #region Methods

    public async Task<PaginatedList<TResponse>> GetUsersAsync<TResponse>(int? UserID, int? roleFilter, int? keywordFilter, Pagination pagination) where TResponse : class
    {
        return await ExecuteSqlStoredProcedureAsync<TResponse>(StoredProceduresLegend.GetUsers, pagination, new List<SqlParameter>()
        {
            new SqlParameter("@userID", UserID <= 0 ? DBNull.Value : UserID),
            new SqlParameter("@roleFilter", roleFilter <= 0 ? DBNull.Value : roleFilter),
            new SqlParameter("@keywordFilter", keywordFilter <= 0 ? DBNull.Value : keywordFilter),

        });
    }

    #endregion
}
