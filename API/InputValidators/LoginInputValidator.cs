using API.Inputs;
using FluentValidation;

namespace API.InputValidators;

public class LoginInputValidator : AbstractValidator<LoginInput>
{
    public LoginInputValidator()
    {
        RuleFor(x => x.ICNumber)
            .NotEmpty().WithMessage("ICNumber is required");
    }
}