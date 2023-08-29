using API.Chargebee.Abstractions;
using API.FirstPromoter.Abstractions;
using API.FirstPromoter.Models.Promoter;
using Application.Common.Constants;
using Application.Common.Extensions;
using Application.Modules.Users.Models;
using AutoMapper;
using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Abstractions.IServices;
using Domain.Common.Constants;
using Domain.Common.Exceptions;
using Domain.ConfigurationOptions;
using MediatR;

namespace Application.Modules.Users.Queries.GetMyUser;

public class GetMyUserQueryHandler : IRequestHandler<GetMyUserQuery, UserDto>
{
    #region CONSTRUCTORS AND LOCALS

    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationOptions _options;
    private readonly IPromoterService _promoterService;
    private readonly ICustomerService _customerService;
    private readonly IChargeBeeService _chargeBeeService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISubscriptionService _subscriptionService;
    public GetMyUserQueryHandler(IMapper mapper,
                                 IUnitOfWork unitOfWork,
                                 ApplicationOptions options,
                                 IPromoterService promoterService,
                                 ICustomerService customerService,
                                 IChargeBeeService chargeBeeService,
                                 ICurrentUserService currentUserService,
                                 ISubscriptionService subscriptionService)
    {
        _mapper = mapper;
        _options = options;
        _unitOfWork = unitOfWork;
        _promoterService = promoterService;
        _customerService = customerService;
        _chargeBeeService = chargeBeeService;
        _currentUserService = currentUserService;
        _subscriptionService = subscriptionService;
    }
    
    #endregion

    public async Task<UserDto> Handle(GetMyUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _chargeBeeService.GetUserNoTrackingAsync(_currentUserService.Email);
        if (!user.IsActive)
        {
            throw new ClientException("Your account is temporarily suspended by admin. Please contact support !", System.Net.HttpStatusCode.BadRequest);
        }

        var userResponse = _mapper.Map<UserDto>(user);

        #region IF CUSTOMER

        if (user.fk_RoleID is RoleLegend.CUSTOMER)
        {
            #region CHARGEBEE

            var chargeBeeCustomer = _customerService.Get(user.ChargeBeeCustomerID, ChargeBeeCustomFieldKeys.Customer);
            var chargeBeeCustomerPaymentSource = _customerService.GetPaymentSource(user.ChargeBeeCustomerID);
            var currentShippingAddress = _subscriptionService.GetShippingAddress(_currentUserService.ChargeBeeSubscriptionID);
            var currentBillingAddress = chargeBeeCustomer.Billing;

            if (currentShippingAddress is not null) userResponse.Shipping = _mapper.Map<UserAddress>(currentShippingAddress);
            if (currentBillingAddress is not null) userResponse.Billing = _mapper.Map<UserAddress>(currentBillingAddress);
            if (chargeBeeCustomerPaymentSource is not null) userResponse.PaymentSource = _mapper.Map<UserPaymentSource>(chargeBeeCustomerPaymentSource);

            #endregion

            #region FIRST PROMOTER

            if (string.IsNullOrEmpty(user.FirstPromoterID))
            {
                var promoter = _promoterService.Create(new CreatePromoter()
                {
                    Email = user?.Email,
                    FirstName = user?.FirstName,
                    LastName = user?.LastName,
                    CustomerID = user?.ChargeBeeCustomerID,
                    CampaignID = "18912", // compaign name = Referral
                    SkipEmailNotification = true,
                });

                user.FirstPromoterID = promoter.ID.ToString();
                user.FirstPromoterAuthToken = promoter.AuthToken;
                user.FirstPromoterReferralID = promoter.DefaultRefID;
            }

            user.FirstPromoterAuthToken = _promoterService.ResetAuthToken(new ResetPromoterToken
            {
                ID = user?.FirstPromoterID,
                CustomerID = user?.ChargeBeeCustomerID,
                AuthToken = user?.FirstPromoterAuthToken,
            }).AuthToken;

            if (string.IsNullOrEmpty(userResponse.FirstPromoterID)) userResponse.FirstPromoterID = user.FirstPromoterID;
            if (string.IsNullOrEmpty(userResponse.FirstPromoterAuthToken)) userResponse.FirstPromoterAuthToken = user.FirstPromoterAuthToken;

            userResponse.FirstPromoterReferralUrl = $"{_options.RegistrationUrl}?fp_ref={user.FirstPromoterReferralID}";

            #endregion

            user.PhoneNumber = user.PhoneNumber.FormatPhoneNumber();
            user.Role = null;
            user.Gender = null;
            _unitOfWork.Users.Update(user);
            _unitOfWork.Complete();
        }

        #endregion

        #region IMPERSONATION

        if (!_currentUserService.LoggedInAs(RoleLegend.CUSTOMER) && _currentUserService.RoleIs(RoleLegend.CUSTOMER))
        {
            var impersonatorUser = await _unitOfWork.Users.GetFirstOrDefaultNoTrackingAsync(x => x.ID == _currentUserService.LoggedInUser && x.fk_RoleID == _currentUserService.LoggedInUserRole && !x.IsDeleted);
            userResponse.Impersonator = new UserImpersonatorDto(impersonatorUser is not null)
            {
                FirstName = impersonatorUser!.FirstName,
                LastName = impersonatorUser!.LastName,
                Email = impersonatorUser!.Email,
                fk_RoleID = impersonatorUser!.fk_RoleID
            };
        }

        #endregion

        return userResponse;
    }
}
