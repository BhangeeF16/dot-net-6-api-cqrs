using API.Chargebee.Abstractions;
using API.Chargebee.Models;
using Domain.Abstractions.IAuth;
using MediatR;

namespace Application.Modules.Users.Commands.UpdateMyAddressBilling
{
    public class UpdateMyAddressBillingCommandHandler : IRequestHandler<UpdateMyAddressBillingCommand, bool>
    {
        private readonly ICustomerService _customerService;
        private readonly ICurrentUserService _currentUserService;
        public UpdateMyAddressBillingCommandHandler(ICustomerService customerService, ICurrentUserService currentUserService) => (_customerService, _currentUserService) = (customerService, currentUserService);

        public async Task<bool> Handle(UpdateMyAddressBillingCommand request, CancellationToken cancellationToken)
        {
            _customerService.Update(_currentUserService.ChargeBeeCustomerID, new ChargeBeeAddress
            {
                FirstName = _currentUserService.FirstName,
                LastName = _currentUserService.LastName,
                Email = _currentUserService.Email,
                PhoneNumber = request.PhoneNumber,
                ExtendedAddr = request.Line1,
                ExtendedAddr2 = request.Line2,
                City = request.City,
                State = request.State,
                Zip = request.PostCode,
                Country = request.Country,
            });

            await Task.CompletedTask;
            return true;
        }
    }
}
