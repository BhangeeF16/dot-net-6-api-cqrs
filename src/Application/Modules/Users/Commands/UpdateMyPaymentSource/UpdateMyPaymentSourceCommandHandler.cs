using API.Chargebee.Abstractions;
using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Exceptions;
using MediatR;
using System.Net;

namespace Application.Modules.Users.Commands.UpdateMyPaymentSource
{
    public class UpdateMyPaymentSourceCommandHandler : IRequestHandler<UpdateMyPaymentSourceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerService _customerService;
        private readonly ICurrentUserService _currentUserService;
        public UpdateMyPaymentSourceCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ICustomerService customerService)
        {
            _unitOfWork = unitOfWork;
            _customerService = customerService;
            _currentUserService = currentUserService;
        }
        public async Task<bool> Handle(UpdateMyPaymentSourceCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.ID;
            var thisUser = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.ID == userId && x.IsActive && !x.IsDeleted);
            if (thisUser is null)
            {
                throw new ClientException("No User Found", HttpStatusCode.NotFound);
            }


            try
            {
                thisUser.ChargeBeePaymentSourceID = _customerService.Update(thisUser.ChargeBeeCustomerID, request.ChargebeeToken).ID;
                _unitOfWork.Complete();
                return true;
            }
            catch (Exception ex)
            {
                throw new ClientException(ex.Message, HttpStatusCode.BadRequest);
            }
        }
    }
}
