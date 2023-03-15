using API.Private.MinimalModule;
using Application.Modules.Lookups.Models;
using Application.Modules.Lookups.Queries.GetCountries;
using Domain.ConfigurationOptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace API.Endpoints;

public class Lookups : BaseModule, IModule
{
    public IServiceCollection RegisterModule(IServiceCollection services)
    {
        return services;
    }
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        #region Lookups

        endpoints.MapGet("/countries", [Authorize]
        async (IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var response = await _mediator.Send(new GetCountriesQuery());
                return Results.Ok(new SuccessResponseModel<List<CountryModel>>
                {
                    Message = "Countries Fetched Successfully",
                    Result = response,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .AddMetaData<List<CountryModel>>
        (
            "Lookups",
            "Gets Available Countries",
            "query-params : none. body-params: none , Returns List of Countries"
        );

        #endregion

        return endpoints;
    }
}
