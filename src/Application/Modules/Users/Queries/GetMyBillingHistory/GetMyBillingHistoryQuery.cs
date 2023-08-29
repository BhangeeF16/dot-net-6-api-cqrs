using Domain.Models.Pagination;
using MediatR;

namespace Application.Modules.Users.Queries.GetMyBillingHistory
{
    public class GetMyBillingHistoryQuery : IRequest<PaginatedList<OrderHistory>>
    {
        public Pagination? Pagination { get; set; }
        public GetMyBillingHistoryQuery(Pagination? pagination) => Pagination = pagination;
    }
}
