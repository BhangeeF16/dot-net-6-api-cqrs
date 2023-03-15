using Application.Modules.Users.Models;
using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Queries.GetUserByID
{
    public class GetUserByIDQuery : IRequest<UserDto>
    {
        public int? UserID { get; set; }

        public GetUserByIDQuery(int? userID)
        {
            UserID = userID;
        }
    }
    public class GetOperatorRequestValidator : AbstractValidator<GetUserByIDQuery>
    {
        public GetOperatorRequestValidator()
        {

            RuleFor(c => c.UserID)
                .NotEmpty().WithMessage("CognitoId is required");

        }
    }
}
