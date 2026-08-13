using AssignmentSystem.API.DTOs.Requests;
using FluentValidation;
namespace AssignmentSystem.API.Validation;
public class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(100);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Role).NotEmpty().Must(r => new[] { "Admin", "Teacher", "Student" }.Contains(r)).WithMessage("Invalid role");
    }
}
