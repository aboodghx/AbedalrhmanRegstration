using API.Inputs;
using FluentValidation;

namespace API.InputValidators;

public class PINCheckInputValidator : AbstractValidator<PINCheckInput>
{
    public PINCheckInputValidator()
    {
        RuleFor(x => x.PIN)
            .Equal(x => x.ConfirmPIN).WithMessage("Incorrect PIN")
            .NotEmpty().WithMessage("PIN is required")
            .MaximumLength(6).WithMessage("The Maximum Length 6");

        RuleFor(x => x.ConfirmPIN)
            .NotEmpty().WithMessage("PIN is required")
            .MaximumLength(6).WithMessage("The Maximum Length 6");
    }
}