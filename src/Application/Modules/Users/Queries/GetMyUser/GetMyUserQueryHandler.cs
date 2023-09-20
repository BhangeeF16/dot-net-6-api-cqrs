using Application.Common.Extensions;
using Application.Modules.Users.Models;
using AutoMapper;
using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Constants;
using Domain.Common.Exceptions;
using Domain.ConfigurationOptions;
using Domain.Entities.UsersModule;
using MediatR;

namespace Application.Modules.Users.Queries.GetMyUser;

public class GetMyUserQueryHandler : IRequestHandler<GetMyUserQuery, UserDto>
{
    #region CONSTRUCTORS AND LOCALS

    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationOptions _options;
    private readonly ICurrentUserService _currentUserService;
    public GetMyUserQueryHandler(IMapper mapper,
                                 IUnitOfWork unitOfWork,
                                 ApplicationOptions options,
                                 ICurrentUserService currentUserService)
    {
        _mapper = mapper;
        _options = options;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    #endregion

    public async Task<UserDto> Handle(GetMyUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultNoTrackingAsync(x => x.Email.ToLower().Equals(_currentUserService.Email.ToLower()) && x.IsActive && !x.IsDeleted) ?? throw new NotFoundException(nameof(User), _currentUserService.Email);
        if (!user.IsActive) throw new BadRequestException("Your account is temporarily suspended by admin. Please contact support !");

        var userResponse = _mapper.Map<UserDto>(user);

        #region IF CUSTOMER

        if (user.fk_RoleID is RoleLegend.USER)
        {
            user.PhoneNumber = user.PhoneNumber.FormatPhoneNumber();
            user.Role = null;
            user.Gender = null;
            _unitOfWork.Users.Update(user);
            _unitOfWork.Complete();
        }

        #endregion

        #region IMPERSONATION

        if (!_currentUserService.LoggedInAs(RoleLegend.USER) && _currentUserService.RoleIs(RoleLegend.USER))
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
