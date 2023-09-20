using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Constants;
using Domain.Common.Exceptions;
using Domain.Entities.UsersModule;
using MediatR;

namespace Application.Modules.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public RegisterUserCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (await _unitOfWork.Users.ExistsAsync(x => x.Email.ToLower().Equals(request.Email.ToLower())))
            {
                throw new BadRequestException("A Customer already exist with " + request.Email + ". Please try a different email address.");
            }

            #region Create User in DB

            var user = new User()
            {
                FirstName = request?.FirstName,
                LastName = request?.LastName,
                Email = request?.Email,
                PhoneNumber = request?.PhoneNumber,
                fk_RoleID = RoleLegend.USER,
            };

            await _unitOfWork.Users.AddAsync(user);
            _unitOfWork.Complete();

            #endregion

            return true;
        }
        catch (Exception e)
        {
            if (e is ArgumentException argEx) throw new ClientException(argEx.Message, System.Net.HttpStatusCode.InternalServerError);
            else if (e is ClientException) throw;
            else throw new ClientException("Un able to Process", System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
