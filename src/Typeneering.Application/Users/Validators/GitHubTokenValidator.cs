using FluentValidation;
using Typeneering.Domain.User.Constraints;

namespace Typeneering.Application.Users.Validators;

public class GitHubTokenValidator : AbstractValidator<string>
{
    public GitHubTokenValidator()
    {
        RuleFor(model => model)
            .MaximumLength(UserConstraints.GithubTokenMaxLength)
            .WithMessage("Invalid GitHub token")
            .NotEmpty()
            .WithMessage("Token cannot be empty");
    }
}
