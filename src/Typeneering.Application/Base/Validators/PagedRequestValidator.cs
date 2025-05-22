using FluentValidation;
using Typeneering.Application.Base.Contracts.Requests;

namespace Typeneering.Application.Base.Validators;

public abstract class PagedRequestValidator<TModel> : AbstractValidator<TModel> where TModel : PagedResultRequest
{
    private readonly int _minPage = 0;
    private readonly int _maxPage = 200;
    public PagedRequestValidator()
    {
        RuleFor(model => model.Skip)
            .GreaterThanOrEqualTo(_minPage)
            .WithMessage("Page can't be negative");

        RuleFor(model => model.Take)
            .LessThanOrEqualTo(_maxPage)
            .WithMessage("Maximum pages allowed per request cannot be over" + _maxPage);
    }
}
