using API.Inputs;
using FluentValidation;

namespace API.InputValidators;

public class BaseInputValidator : AbstractValidator<BaseInput>
{
    public BaseInputValidator()
    {
        RuleFor(x => x.RequestId)
                   .NotEmpty().WithMessage("UserId is required")
                   .NotEqual(Guid.Empty).WithMessage("UserId cannot be an empty GUID");
    }
}