using Domain.Abstractions.IRepositories.IEntity;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Constants;
using Domain.ConfigurationOptions;
using Domain.Entities.LookupsModule;
using Domain.Models.Pagination;
using Infrastructure.DataAccess.GenericRepositories;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;

namespace Infrastructure.DataAccess.EntityRepositories;

public class LookupsRepository : ILookupsRepository
{
    private readonly ApplicationDbContext _context;
    private readonly InfrastructureOptions _infrastructureOptions;
    public LookupsRepository(ApplicationDbContext context, InfrastructureOptions infrastructureOptions) => (_context, _infrastructureOptions) = (context, infrastructureOptions);

    #region Inner-Repositories

    private IGenericRepository<Day>? _day;
    private IGenericRepository<State>? _states;
    private IGenericRepository<Gender>? _genders;
    private IGenericRepository<Carrier>? _carrier;
    private IGenericRepository<TimeSlot>? _timeSlot;
    private IGenericRepository<Country>? _countries;

    public IGenericRepository<Day> Days => _day ??= new GenericRepository<Day>(_context, _infrastructureOptions);
    public IGenericRepository<State> States => _states ??= new GenericRepository<State>(_context, _infrastructureOptions);
    public IGenericRepository<Carrier> Carriers => _carrier ??= new GenericRepository<Carrier>(_context, _infrastructureOptions);
    public IGenericRepository<Country> Countries => _countries ??= new GenericRepository<Country>(_context, _infrastructureOptions);
    public IGenericRepository<TimeSlot> TimeSlots => _timeSlot ??= new GenericRepository<TimeSlot>(_context, _infrastructureOptions);
    public IGenericRepository<Gender> Genders => _genders ??= new GenericRepository<Gender>(_context, _infrastructureOptions);

    #endregion

    #region Methods

    public async Task<PaginatedList<TResponse>> GetStatesAsync<TResponse>(Pagination pagination) where TResponse : class => await States.ExecuteSqlStoredProcedureAsync<TResponse>(StoredProceduresLegend.GetStates, pagination, new List<SqlParameter>());

    #endregion
}
