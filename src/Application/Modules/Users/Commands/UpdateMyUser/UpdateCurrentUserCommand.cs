using Application.Modules.Users.Models;
using Domain.Common.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Reflection;

namespace Application.Modules.Users.Commands.UpdateMyUser
{
    public class UpdateCurrentUserCommand : IRequest<UserDto>
    {
        [JsonProperty("firstName")]
        public string? FirstName { get; set; }
        [JsonProperty("lastName")]
        public string? LastName { get; set; }
        [JsonProperty("email")]
        public string? Email { get; set; }
        [JsonProperty("phoneNumber")]
        public string? PhoneNumber { get; set; }

        //[JsonProperty("dob")]
        //public DateTime? DOB { get; set; }
        //[JsonProperty("gender")]
        //public int? Gender { get; set; }
        //[JsonProperty("address")]
        //public string? Address { get; set; }
        //[JsonProperty("image")]
        //public IFormFile? Image { get; set; }

        public static ValueTask<UpdateCurrentUserCommand?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            var FormData = context.Request.Form;

            var result = new UpdateCurrentUserCommand
            {
                FirstName = FormData["firstName"],
                LastName = FormData["lastName"],
                Email = FormData["email"],
                PhoneNumber = FormData["phoneNumber"],
                //Address = FormData["address"],
                //DOB = DateTime.Parse(FormData["dob"]),
                //Image = FormData.Files.Any() ? FormData.Files[0] : null
            };

            return ValueTask.FromResult<UpdateCurrentUserCommand?>(result);
        }
        public class Validator : AbstractValidator<UpdateCurrentUserCommand>
        {
            public Validator()
            {
                RuleFor(c => c.FirstName).ValidateProperty();
                RuleFor(c => c.LastName).ValidateProperty();
                RuleFor(c => c.PhoneNumber).ValidateProperty();
                RuleFor(c => c.Email).ValidateProperty().EmailAddress().WithMessage("Email is required");
            }
        }
    }
}
