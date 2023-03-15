using Application.Modules.Users.Models;
using AutoMapper;
using Domain.Common.Exceptions;
using Domain.IContracts.IAuth;
using Domain.IContracts.IRepositories.IGenericRepositories;
using MediatR;
using System.Net;

namespace Application.Modules.Users.Commands.UpdateMyUser
{
    public class UpdateCurrentUserCommandHandler : IRequestHandler<UpdateCurrentUserCommand, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public UpdateCurrentUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<UserDto> Handle(UpdateCurrentUserCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.ID;
            var thisUser = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.ID == userId && x.IsActive == true && x.IsDeleted == false);
            if (thisUser == null)
            {
                throw new ClientException("No User Found", HttpStatusCode.NotFound);
            }
            else
            {
                thisUser.FirstName = request.FirstName;
                thisUser.LastName = request.LastName;
                thisUser.Email = request.Email;
                thisUser.PhoneNumber = request.PhoneNumber;
                //thisUser.DOB = request.DOB;
                //thisUser.fk_GenderID = request.Gender;
                //thisUser.Address = request.Address;
                //if (request.Image != null)
                //{
                //    var key = _fileUploadService.UploadFile(request.Image);
                //    thisUser.ImageKey = string.IsNullOrEmpty(key) ? thisUser.ImageKey : key;
                //}

                _unitOfWork.Complete();
            }
            var response = _mapper.Map<UserDto>(thisUser);
            //if (!string.IsNullOrEmpty(thisUser.ImageKey))
            //{
            //    response.ImageKey = _fileUploadService.GetFileCompleteUrl(thisUser.ImageKey);
            //}
            return response;
        }
    }
}
