using Application.Modules.Lookups.Models;
using AutoMapper;
using Domain.IContracts.IRepositories.IGenericRepositories;
using MediatR;

namespace Application.Modules.Lookups.Queries.GetCountries
{
    public class GetCountriesQueryHandler : IRequestHandler<GetCountriesQuery, List<CountryModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetCountriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<CountryModel>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
        {
            var countries = await _unitOfWork.Users.Countries.GetAllAsync();
            return _mapper.Map<List<CountryModel>>(countries);
        }
    }
}
