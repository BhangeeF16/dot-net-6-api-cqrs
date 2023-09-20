using Application.Modules.Users.Models;
using AutoMapper;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Exceptions;
using MediatR;

namespace Application.Modules.Users.Queries.GetUserByID;

public class GetUserByIDQueryHandler : IRequestHandler<GetUserByIDQuery, UserDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    public GetUserByIDQueryHandler(IMapper mapper, IUnitOfWork unitOfWork) => (_mapper, _unitOfWork) = (mapper, unitOfWork);

    public async Task<UserDto> Handle(GetUserByIDQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.ID == request.UserID && x.IsActive && !x.IsDeleted, x => x.Role, x => x.Gender) ?? throw new NotFoundException("Unable to Find User !! ");
        return _mapper.Map<UserDto>(user);
    }
}
