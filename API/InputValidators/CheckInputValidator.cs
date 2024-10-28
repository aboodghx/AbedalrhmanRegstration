using API.Inputs;
using FluentValidation;

namespace API.InputValidators;

public class CheckInputValidator : AbstractValidator<CheckInput>
{
    public CheckInputValidator()
    {
        Include(new BaseInputValidator());

        RuleFor(x => x.Code)
           .NotEmpty().WithMessage("Code is required")
           .MaximumLength(4).WithMessage("The Maximum Length 4"); ;
    }
}