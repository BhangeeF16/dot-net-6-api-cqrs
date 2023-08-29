using API.FirstPromoter.Abstractions;
using API.FirstPromoter.Models.Promoter;
using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Abstractions.IServices;
using MediatR;

namespace Application.Modules.Users.Commands.StartReferring;

public class StartReferringCommandHandler : IRequestHandler<StartReferringCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPromoterService _promoterService;
    private readonly IChargeBeeService _chargeBeeService;
    private readonly ICurrentUserService _currentUserService;
    public StartReferringCommandHandler(IUnitOfWork unitOfWork, IPromoterService promoterService, IChargeBeeService chargeBeeService, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _promoterService = promoterService;
        _chargeBeeService = chargeBeeService;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(StartReferringCommand request, CancellationToken cancellationToken)
    {
        var user = await _chargeBeeService.GetUserNoTrackingAsync(_currentUserService.ChargeBeeCustomerID, false);

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

        var FirstPromoterAuthToken = _promoterService.ResetAuthToken(new ResetPromoterToken
        {
            ID = user?.FirstPromoterID,
            CustomerID = user?.ChargeBeeCustomerID,
            AuthToken = user?.FirstPromoterAuthToken,
        }).AuthToken;

        if (string.IsNullOrEmpty(FirstPromoterAuthToken))
        {
            user.FirstPromoterAuthToken = FirstPromoterAuthToken;
        }

        user.IsReferringStarted = true;

        _unitOfWork.Users.Update(user);
        _unitOfWork.Complete();

        return true;

    }
}
