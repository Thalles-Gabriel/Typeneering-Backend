using FluentValidation;
using Typeneering.Application.UserPreferences.Contracts.Requests;
using Typeneering.Domain.Preference.Constraints;

namespace Typeneering.Application.UserPreferences.Validators;

public class PostUserPreferenceValidator : AbstractValidator<PostUserPreferenceRequest>
{
    public PostUserPreferenceValidator()
    {
        RuleFor(model => model.Value)
            .MaximumLength(UserPreferenceConstraints.ValueMaxLength)
            .WithMessage("The value is too long");
    }
}
