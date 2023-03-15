using Domain.Entities.UsersModule;
using Domain.IContracts.IRepositories.IGenericRepositories;

namespace Domain.IContracts.IRepositories.IEntityRepositories;
public interface IUserRepository : IGenericRepository<User>
{
    IGenericRepository<UserLoginHistory> LoginHistories { get; }
    IGenericRepository<Country> Countries { get; }
    IGenericRepository<State> States { get; }
    IGenericRepository<Gender> Genders { get; }
}
