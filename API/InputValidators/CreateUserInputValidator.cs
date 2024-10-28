using API.Inputs;
using FluentValidation;

namespace API.InputValidators;

public class CreateUserInputValidator : AbstractValidator<CreateUserInput>
{
    public CreateUserInputValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("The provided email format is invalid. Please enter a valid email address");

        RuleFor(x => x.ICNumber)
            .NotEmpty().WithMessage("ICNumber is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(14)
            .Matches("^[0-9]+$").WithMessage("Only Numbers")
            .NotEmpty().WithMessage("Phone Number is required");
    }
}