using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Entities.LookupsModule;

namespace Domain.Abstractions.IRepositories.IEntity;
public interface ILookupsRepository
{
    IGenericRepository<Gender> Genders { get; }

}