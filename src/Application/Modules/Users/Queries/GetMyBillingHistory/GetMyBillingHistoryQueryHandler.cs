using API.Chargebee.Abstractions;
using AutoMapper;
using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Extensions;
using Domain.Models.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Users.Queries.GetMyBillingHistory;

public class GetMyBillingHistoryQueryHandler : IRequestHandler<GetMyBillingHistoryQuery, PaginatedList<OrderHistory>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomerService _customerService;
    private readonly ICurrentUserService _currentUserService;
    public GetMyBillingHistoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService, ICustomerService customerService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _customerService = customerService;
        _currentUserService = currentUserService;
    }
    public async Task<PaginatedList<OrderHistory>> Handle(GetMyBillingHistoryQuery request, CancellationToken cancellationToken)
    {
        try
        {

            var UserId = _currentUserService.ID;
            var thisUser = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.ID == UserId && x.IsActive && !x.IsDeleted);
            var histories = _customerService.GetBillingHistory(thisUser.ChargeBeeCustomerID);

            var billingHistories = _mapper.Map<List<OrderHistory>>(histories.ToList());
            var response = await billingHistories.AsQueryable().PaginatedListAsync(request.Pagination.PageNumber ?? 1, request.Pagination.PageSize ?? 10);

            var currentPlanIds = response.Items.Select(x => x.Plan.ChargebeeID).ToArray();
            var currentPlanVariations = await _unitOfWork.Plans.Variations.GetWhereAsync(x => currentPlanIds.Contains(x.ChargeBeeID), x => x.Plan);
            var currentPantryItemIDs = response.Items.SelectMany(x => x.PantryItems).Select(x => x.ChargebeeID).ToArray();
            var currentPantryItemVariations = await _unitOfWork.PantryItems.Variations.GetWhereAsync(x => currentPantryItemIDs.Contains(x.ChargeBeeID), x => x.PantryItem);

            foreach (var history in response.Items)
            {
                var planVariation = currentPlanVariations.FirstOrDefault(x => history.Plan.ChargebeeID == x.ChargeBeeID);
                if (planVariation is not null)
                {
                    history.Plan.ID = planVariation.fk_PlanID;
                    history.Plan.InvoiceName = planVariation.InvoiceName;
                    history.Plan.Name = planVariation.Plan.Name;
                }

                if (currentPantryItemVariations != null && history.PantryItems != null && history.PantryItems.Any())
                {
                    var extras = new List<OrderHistoryPantryItem>();
                    foreach (var item in history.PantryItems)
                    {
                        if (currentPantryItemVariations.Any(y => item.ChargebeeID.Equals(y.ChargeBeeID)))
                        {
                            var x = currentPantryItemVariations.FirstOrDefault(y => item.ChargebeeID.Equals(y.ChargeBeeID));
                            extras.Add(new OrderHistoryPantryItem
                            {
                                ID = item.ID,
                                Name = x.PantryItem.Name,
                                InvoiceName = x.InvoiceName,
                                Qty = item.Qty,
                                Frequency = x.Frequency,
                                ShippingFrequency = x.ShippingFrequency,
                            });
                        }
                    }

                    history.PantryItems = extras.ToArray();
                }


            }

            return response;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
