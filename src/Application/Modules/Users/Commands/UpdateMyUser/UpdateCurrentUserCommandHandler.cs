using Application.Common.Extensions;
using Application.Modules.Users.Models;
using AutoMapper;
using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Exceptions;
using MediatR;
using System.Net;

namespace Application.Modules.Users.Commands.UpdateMyUser;

public class UpdateCurrentUserCommandHandler : IRequestHandler<UpdateCurrentUserCommand, UserDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    public UpdateCurrentUserCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }
    public async Task<UserDto> Handle(UpdateCurrentUserCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.ID;
        var thisUser = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.ID == userId && x.IsActive && !x.IsDeleted);
        if (thisUser == null)
        {
            throw new ClientException("No User Found", HttpStatusCode.NotFound);
        }
        else
        {
            thisUser.FirstName = request.FirstName;
            thisUser.LastName = request.LastName;
            thisUser.Email = request.Email;
            thisUser.PhoneNumber = request.PhoneNumber.FormatPhoneNumber();

            _unitOfWork.Complete();
        }

        return _mapper.Map<UserDto>(thisUser);
    }
}
