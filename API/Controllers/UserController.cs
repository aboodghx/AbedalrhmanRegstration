using API.Inputs;
using API.InputValidators;
using App.Models;
using App.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService userService;
    private readonly IValidator<CheckInput> checkInputValidator;
    private readonly IValidator<BaseInput> baseInputValidator;
    private readonly IValidator<LoginInput> loginInputValidator;
    private readonly IValidator<PINCheckInput> pinCheckInput;
    private readonly IValidator<CreateUserInput> createUserValidator;

    public UserController(
        IUserService userService,
        IValidator<CheckInput> checkInputValidator,
        IValidator<BaseInput> baseInputValidator,
        IValidator<LoginInput> loginInputValidator,
        IValidator<PINCheckInput> pinCheckInput,
        IValidator<CreateUserInput> createUserValidator)
    {
        this.userService = userService;
        this.checkInputValidator = checkInputValidator;
        this.baseInputValidator = baseInputValidator;
        this.loginInputValidator = loginInputValidator;
        this.pinCheckInput = pinCheckInput;
        this.createUserValidator = createUserValidator;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(CheckResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginInput input, CancellationToken cancellationToken)
    {
        var result = await userService.Login(input.ICNumber, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CheckResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateRequest([FromBody] CreateUserInput input, CancellationToken cancellationToken)
    {
        var result = await userService.CreateRequest(input.Name, input.Email, input.ICNumber, input.PhoneNumber, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost("phone-number-check")]
    [ProducesResponseType(typeof(CheckResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> PhoneNumberCheck([FromBody] CheckInput input, CancellationToken cancellationToken)
    {
        var result = await userService.PhoneNumberCheck(input.RequestId, input.Code, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost("email-check")]
    [ProducesResponseType(typeof(CheckResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> EmailCheck([FromBody] CheckInput input, CancellationToken cancellationToken)
    {
        var result = await userService.EmailCheck(input.RequestId, input.Code, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost("policy-check")]
    [ProducesResponseType(typeof(CheckResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> PolicyCheck([FromBody] BaseInput input, CancellationToken cancellationToken)
    {
        var result = await userService.PolicyCheck(input.RequestId, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost("pin-check")]
    [ProducesResponseType(typeof(CheckResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> PolicyCheck([FromBody] PINCheckInput input, CancellationToken cancellationToken)
    {
        var result = await userService.PINCheck(input.RequestId, input.PIN, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost("biomtric-check")]
    [ProducesResponseType(typeof(CheckResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> BiomtricCheck([FromBody] BaseInput input, CancellationToken cancellationToken)
    {
        var result = await userService.BiomtricCheck(input.RequestId, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}