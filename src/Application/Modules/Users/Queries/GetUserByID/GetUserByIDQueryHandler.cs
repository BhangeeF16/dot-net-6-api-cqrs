using Application.Modules.Users.Models;
using AutoMapper;
using Domain.IContracts.IAuth;
using Domain.IContracts.IRepositories.IGenericRepositories;
using MediatR;

namespace Application.Modules.Users.Queries.GetUserByID
{
    public class GetUserByIDQueryHandler : IRequestHandler<GetUserByIDQuery, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public GetUserByIDQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<UserDto> Handle(GetUserByIDQuery request, CancellationToken cancellationToken)
        {
            var DoesDiverUserExist = await _unitOfWork.Users.ExistsAsync(x => x.ID == request.UserID && x.IsActive == true && x.IsDeleted == false);
            if (DoesDiverUserExist)
            {
                var thisUser = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.ID == request.UserID && x.IsActive == true && x.IsDeleted == false, x => x.Role);
                var userResponse = _mapper.Map<UserDto>(thisUser);

                //userResponse.ImageKey = !string.IsNullOrEmpty(userResponse.ImageKey) ? _s3Service.GetAWSFileURL(userResponse.ImageKey) : userResponse.ImageKey;
                return userResponse;
            }
            else
            {
                throw new Domain.Common.Exceptions.ClientException("Unable to Find User !! ", System.Net.HttpStatusCode.NotFound);
            }
        }
    }
}
