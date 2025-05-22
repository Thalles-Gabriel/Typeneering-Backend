using FluentValidation;
using Typeneering.Application.Base.Constants;
using Typeneering.Application.Base.Validators;
using Typeneering.Application.Sessions.Contracts.Requests;
using Typeneering.Application.Users.Constants;

namespace Typeneering.Application.Sessions.Validators;

public class GetSessionValidator : PagedRequestValidator<GetSessionRequest>
{
    public GetSessionValidator() : base()
    {
        RuleFor(model => model.MaxCharacters)
            .GreaterThanOrEqualTo(0)
            .WithMessage(BaseConsts.Validations.NegativeIntMessage);

        RuleFor(model => model.MaxCorrectCharacters)
            .GreaterThanOrEqualTo(0)
            .WithMessage(BaseConsts.Validations.NegativeIntMessage);

        RuleFor(model => model.Seconds)
            .GreaterThanOrEqualTo(0)
            .WithMessage(BaseConsts.Validations.NegativeIntMessage);

        RuleFor(model => model.MinCharacters)
            .GreaterThanOrEqualTo(0)
            .WithMessage(BaseConsts.Validations.NegativeIntMessage);

        RuleFor(model => model.MinCorrectCharacters)
            .GreaterThanOrEqualTo(0)
            .WithMessage(BaseConsts.Validations.NegativeIntMessage);

        RuleFor(model => model.EndDate)
            .GreaterThan(DateTimeOffset.UtcNow)
            .WithMessage(BaseConsts.Validations.FutureDateTimeMessage);
    }
}
