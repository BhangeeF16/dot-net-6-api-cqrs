using Application.Common.Extensions;
using Application.Modules.Users.Models;
using AutoMapper;
using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Exceptions;
using MediatR;

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
        var user = await _unitOfWork.Users.GetFirstOrDefaultNoTrackingAsync(x => x.ID == userId && x.IsActive && !x.IsDeleted) ?? throw new NotFoundException("No User Found");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber.FormatPhoneNumber();
        _unitOfWork.Users.Update(user);
        _unitOfWork.Complete();

        return _mapper.Map<UserDto>(user);
    }
}
