using AssignmentSystem.API.DTOs.Requests;
using FluentValidation;
namespace AssignmentSystem.API.Validation;
public class GradeSubmissionValidator : AbstractValidator<GradeSubmissionRequest>
{
    public GradeSubmissionValidator()
    {
        RuleFor(x => x.Marks).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Status).NotEmpty().Must(s => new[] { "Graded", "Rejected" }.Contains(s));
    }
}
