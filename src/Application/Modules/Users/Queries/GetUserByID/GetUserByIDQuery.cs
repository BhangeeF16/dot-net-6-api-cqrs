using Application.Modules.Users.Models;
using Domain.Common.Extensions;
using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Queries.GetUserByID
{
    public class GetUserByIDQuery : IRequest<UserDto>
    {
        public int? UserID { get; set; }
        public GetUserByIDQuery(int? userID) => UserID = userID;

        public class Validator : AbstractValidator<GetUserByIDQuery>
        {
            public Validator() => RuleFor(c => c.UserID).ValidateProperty();
        }
    }
}
