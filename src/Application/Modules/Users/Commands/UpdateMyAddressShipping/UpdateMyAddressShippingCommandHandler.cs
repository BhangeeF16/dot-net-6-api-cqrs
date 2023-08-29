using API.Chargebee.Abstractions;
using API.Chargebee.Models;
using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Abstractions.IServices;
using Domain.Common.Exceptions;
using Domain.Common.Extensions;
using MediatR;
using Twilio.TwiML.Voice;

namespace Application.Modules.Users.Commands.UpdateMyAddressShipping
{
    public class UpdateMyAddressShippingCommandHandler : IRequestHandler<UpdateMyAddressShippingCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChargeBeeService _chargeBeeService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ISubscriptionService _subscriptionService;
        public UpdateMyAddressShippingCommandHandler(ICurrentUserService currentUserService, ISubscriptionService subscriptionService, IUnitOfWork unitOfWork, IChargeBeeService chargeBeeService)
        {
            _unitOfWork = unitOfWork;
            _chargeBeeService = chargeBeeService;
            _currentUserService = currentUserService;
            _subscriptionService = subscriptionService;
        }

        public async Task<bool> Handle(UpdateMyAddressShippingCommand request, CancellationToken cancellationToken)
        {
            var UserId = _currentUserService.ID;
            var userSubscription = await _chargeBeeService.GetUserSubscriptionNoTrackingAsync(_currentUserService.ChargeBeeCustomerID, _currentUserService.ID);
            var userSubscriptionNext = await _unitOfWork.Users.SubscriptionSettings.GetFirstOrDefaultAsync(x => x.ID == userSubscription.fk_NextSettingID, x => x.PostCodeDeliverySchedule, x => x.PostCodeDeliverySchedule.PostCode);

            if (request.PostCode.IsNotEqualTo(userSubscriptionNext!.PostCodeDeliverySchedule!.PostCode!.PostalCode!))
            {
                throw new ClientException($"Schedule post code ({userSubscriptionNext!.PostCodeDeliverySchedule!.PostCode!.PostalCode!}) must match with the address post code ({request.PostCode!})", System.Net.HttpStatusCode.BadRequest);
            }

            _subscriptionService.Update(_currentUserService.ChargeBeeSubscriptionID, new ChargeBeeAddress
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

            return true;
        }
    }
}
