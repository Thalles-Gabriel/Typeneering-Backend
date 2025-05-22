using FluentValidation;
using Typeneering.Application.Base.Constants;
using Typeneering.Application.Sessions.Contracts.Requests;
using Typeneering.Domain.Session.Constraints;

namespace Typeneering.Application.Sessions.Validators;

public class PostSessionValidator : AbstractValidator<PostSessionRequest>
{
    public PostSessionValidator()
    {
        RuleFor(model => model.Filename)
            .MaximumLength(SessionConstraints.FilenameMaxLength)
            .WithMessage(BaseConsts.Validations.LongStringMessage);

        RuleFor(model => model.Filetype)
            .MaximumLength(SessionConstraints.FiletypeMaxLength)
            .WithMessage(BaseConsts.Validations.LongStringMessage);

        RuleFor(model => model.Seconds)
            .GreaterThanOrEqualTo(0)
            .WithMessage(BaseConsts.Validations.NegativeIntMessage);

        RuleFor(model => model.CorrectCharacters)
            .GreaterThanOrEqualTo(0)
            .WithMessage(BaseConsts.Validations.NegativeIntMessage);

        RuleFor(model => model.CharactersTyped)
            .GreaterThanOrEqualTo(0)
            .WithMessage(BaseConsts.Validations.NegativeIntMessage);

        RuleFor(model => model.TotalCharacters)
            .GreaterThanOrEqualTo(0)
            .WithMessage(BaseConsts.Validations.NegativeIntMessage);
    }
}
