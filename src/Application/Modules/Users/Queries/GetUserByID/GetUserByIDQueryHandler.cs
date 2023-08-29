using Application.Modules.Users.Models;
using AutoMapper;
using Domain.Abstractions.IRepositories.IGeneric;
using MediatR;

namespace Application.Modules.Users.Queries.GetUserByID;

public class GetUserByIDQueryHandler : IRequestHandler<GetUserByIDQuery, UserDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    public GetUserByIDQueryHandler(IMapper mapper, IUnitOfWork unitOfWork) => (_mapper, _unitOfWork) = (mapper, unitOfWork);

    public async Task<UserDto> Handle(GetUserByIDQuery request, CancellationToken cancellationToken)
    {
        var DoesDiverUserExist = await _unitOfWork.Users.ExistsAsync(x => x.ID == request.UserID && x.IsActive && !x.IsDeleted);
        if (DoesDiverUserExist)
        {
            var thisUser = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.ID == request.UserID && x.IsActive && !x.IsDeleted, x => x.Role);
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
