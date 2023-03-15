using API.Chargebee.Services.Customers;
using API.Chargebee.Services.Subscriptions;
using Application.Common.Constants;
using Application.Common.Extensions;
using Application.Modules.Users.Models;
using AutoMapper;
using Domain.Common.Exceptions;
using Domain.Common.Extensions;
using Domain.Entities.UsersModule;
using Domain.IContracts.IAuth;
using Domain.IContracts.IRepositories.IGenericRepositories;
using Domain.IContracts.IServices;
using MediatR;

namespace Application.Modules.Users.Queries.GetMyUser;

public class GetMyUserQueryHandler : IRequestHandler<GetMyUserQuery, UserDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChargeBeeService _chargeBeeService;
    private readonly ICustomerService _customerService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly ICurrentUserService _currentUserService;
    public GetMyUserQueryHandler(ICurrentUserService currentUserService,
                                 IChargeBeeService chargeBeeService,
                                 IUnitOfWork unitOfWork,
                                 IMapper mapper,
                                 ICustomerService customerService,
                                 ISubscriptionService subscriptionService)
    {
        _currentUserService = currentUserService;
        _chargeBeeService = chargeBeeService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _customerService = customerService;
        _subscriptionService = subscriptionService;
    }
    public async Task<UserDto> Handle(GetMyUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.ID == _currentUserService.ID && x.IsActive == true && x.IsDeleted == false, x => x.Role, x => x.Gender, x => x.State);
        if (user is null)
        {
            user ??= await _chargeBeeService.CreateUserFromChargeBeeAsync(_currentUserService.Email);
            if (user == null)
            {
                throw new ClientException("No User Found !", System.Net.HttpStatusCode.BadRequest);
            }
        }

        var userResponse = _mapper.Map<UserDto>(user);

        if (user.fk_RoleID is not 1)
        {
            var chargeBeeCustomer = _customerService.Get(user.ChargeBeeCustomerID, ChargeBeeCustomFieldKeys.Customer);
            var chargeBeeCustomerPaymentSource = _customerService.GetPaymentSource(user.ChargeBeeCustomerID);
            var currentShippingAddress = _subscriptionService.GetShippingAddress(_currentUserService.ChargeBeeSubscriptionID);
            var currentBillingAddress = _customerService.GetBillingAddress(_currentUserService.ChargeBeeCustomerID);

            userResponse.Shipping = new UserAddress
            {
                PhoneNumber = chargeBeeCustomer.CustomFields.GetCustomValue("cf_next_shipping_phone", currentShippingAddress.PhoneNumber),
                Line1 = chargeBeeCustomer.CustomFields.GetCustomValue("cf_next_shipping_address_line_1", currentShippingAddress.ExtendedAddr),
                Line2 = chargeBeeCustomer.CustomFields.GetCustomValue("cf_next_shipping_address_line_2", currentShippingAddress.ExtendedAddr2),
                City = chargeBeeCustomer.CustomFields.GetCustomValue("cf_next_shipping_city", currentShippingAddress.City),
                PostCode = chargeBeeCustomer.CustomFields.GetCustomValue("cf_next_shipping_zip", currentShippingAddress.Zip),
                State = chargeBeeCustomer.CustomFields.GetCustomValue("cf_next_shipping_state", currentShippingAddress.State),
                Country = "Australia",
            };
            userResponse.Billing = new UserAddress
            {
                PhoneNumber = chargeBeeCustomer.CustomFields.GetCustomValue("cf_next_billing_phone", currentBillingAddress.PhoneNumber),
                Line1 = chargeBeeCustomer.CustomFields.GetCustomValue("cf_next_billing_address_line_1", currentBillingAddress.ExtendedAddr),
                Line2 = chargeBeeCustomer.CustomFields.GetCustomValue("cf_next_billing_address_line_2", currentBillingAddress.ExtendedAddr2),
                City = chargeBeeCustomer.CustomFields.GetCustomValue("cf_next_billing_city", currentBillingAddress.City),
                PostCode = chargeBeeCustomer.CustomFields.GetCustomValue("cf_next_billing_zip", currentBillingAddress.Zip),
                State = chargeBeeCustomer.CustomFields.GetCustomValue("cf_next_billing_state", currentBillingAddress.State),
                Country = "Australia",
            };
            userResponse.PaymentSource = new UserPaymentSource
            {
                Last4 = chargeBeeCustomerPaymentSource.Last4,
                Brand = (chargeBeeCustomerPaymentSource.Brand ?? API.Chargebee.Common.PaymentSourceBrand.UnKnown).GetDescription()
            };
        }

        return userResponse;
    }
}
