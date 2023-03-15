using Application.Modules.Lookups.Models;
using MediatR;

namespace Application.Modules.Lookups.Queries.GetCountries
{
    public class GetCountriesQuery : IRequest<List<CountryModel>>
    {

    }
}
