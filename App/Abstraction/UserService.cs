using App.Models;
using App.Services;
using Identity.Models;
using Identity.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Identity.Abstraction;

internal class UserService : IUserService
{
    private readonly DatabaseService databaseService;

    public UserService(DatabaseService databaseService)
    {
        this.databaseService = databaseService;
    }

    public async Task<CheckResponse> Login(string icNumber, CancellationToken cancellationToken)
    {
        var user = await databaseService.Users.FirstOrDefaultAsync(x => x.ICNumber == icNumber, cancellationToken);

        if (user is null)
        {
            return null;
        }

        if (user.IsVerifyEmail || user.IsVerifyPIN || user.IsVerifyBiometric || user.IsVerifyPolicy || user.IsVerifyPhoneNumber)
        {
            var prossesResult = await ProcessSteps(icNumber, cancellationToken);

            if (prossesResult is null || prossesResult.Count == 0)
            {
                return new CheckResponse
                {
                    NextStep = "Profile Verified",
                    RequestId = Guid.Empty,
                };
            }

            var currentStep = prossesResult.Find(x => x.Status == FlowStepStatus.InProgress);

            return new CheckResponse
            {
                NextStep = currentStep.Type.ToString(),
                RequestId = currentStep.RequestId,
            };
        }

        var flowSteps = await GetSteps(cancellationToken);

        await CreateRequest(user.Name, user.Email, user.ICNumber, user.PhoneNumber, RequestType.Login, flowSteps, cancellationToken);

        return new CheckResponse
        {
            NextStep = flowSteps.Find(x => x.Status == FlowStepStatus.InProgress).Type.ToString(),
            RequestId = flowSteps[0].RequestId,
        };
    }

    public async Task<CheckResponse> CreateRequest(string name, string email, string icNumber, string phoneNumber, CancellationToken cancellationToken)
    {
        var requestFlowStpes = await GetSteps(cancellationToken);

        if (requestFlowStpes.TrueForAll(x => x.IsCompleted && x.Status != FlowStepStatus.InProgress && x.Status != FlowStepStatus.Failed))
        {
            return null;
        }

        var request = new Request()
        {
            PhoneNumber = phoneNumber,
            Name = name,
            Email = email,
            ICNumber = icNumber,
            RequestType = RequestType.Registration,
            RequestFlowStpes = requestFlowStpes
        };

        var step = request.RequestFlowStpes.Find(x => x.Status == FlowStepStatus.InProgress);

        if (step is null)
        {
            return null;
        }

        databaseService.Requests.Add(request);

        await databaseService.SaveChangesAsync(cancellationToken);

        return new CheckResponse()
        {
            RequestId = request.Id,
            NextStep = step.Type.ToString(),
        };
    }

    public async Task<CheckResponse> PhoneNumberCheck(Guid requestId, string code, CancellationToken cancellationToken)
    {
        var request = await databaseService
            .Requests
            .Include(x => x.RequestFlowStpes)
            .FirstOrDefaultAsync(x => x.Id == requestId, cancellationToken);

        if (request is null)
        {
            return null;
        }

        var currentStep = GetCurrentStep(request.RequestFlowStpes, StepType.PhoneNumberConfrmation);

        if (currentStep is null)
        {
            return null;
        }

        if (code != "1234")
        {
            return null;
        }

        request.UpdateCurrentStep(currentStep.Type);

        request.UpdateNextStep(StepType.EmailConfrmation);

        await databaseService.SaveChangesAsync(cancellationToken);

        return new CheckResponse()
        {
            NextStep = StepType.EmailConfrmation.ToString(),
            RequestId = request.Id
        };
    }

    public async Task<CheckResponse> EmailCheck(Guid requestId, string code, CancellationToken cancellationToken)
    {
        var request = await databaseService
            .Requests
            .Include(x => x.RequestFlowStpes)
            .FirstOrDefaultAsync(x => x.Id == requestId, cancellationToken);

        if (request is null)
        {
            return null;
        }

        var currentStep = GetCurrentStep(request.RequestFlowStpes, StepType.EmailConfrmation);

        if (currentStep is null)
        {
            return null;
        }

        if (code != "1234")
        {
            currentStep.Status = FlowStepStatus.Failed;
            currentStep.IsCompleted = false;
        }

        request.UpdateCurrentStep(currentStep.Type);

        request.UpdateNextStep(StepType.PolicyConfirmation);

        await databaseService.SaveChangesAsync(cancellationToken);

        return new CheckResponse()
        {
            NextStep = StepType.PolicyConfirmation.ToString(),
            RequestId = request.Id
        };
    }

    public async Task<CheckResponse> PolicyCheck(Guid requestId, CancellationToken cancellationToken)
    {
        var request = await databaseService
            .Requests
            .Include(x => x.RequestFlowStpes)
            .FirstOrDefaultAsync(x => x.Id == requestId, cancellationToken);

        if (request is null)
        {
            return null;
        }

        var currentStep = GetCurrentStep(request.RequestFlowStpes, StepType.PolicyConfirmation);

        if (currentStep is null)
        {
            return null;
        }

        request.UpdateCurrentStep(currentStep.Type);

        request.UpdateNextStep(StepType.PINConfirmation);

        await databaseService.SaveChangesAsync(cancellationToken);

        return new CheckResponse()
        {
            NextStep = StepType.PINConfirmation.ToString(),
            RequestId = request.Id
        };
    }

    public async Task<CheckResponse> PINCheck(Guid requestId, string pin, CancellationToken cancellationToken)
    {
        var request = await databaseService
            .Requests
            .Include(x => x.RequestFlowStpes)
            .FirstOrDefaultAsync(x => x.Id == requestId, cancellationToken);

        if (request is null)
        {
            return null;
        }

        request.PIN = PasswordHasher.HashPassword(pin);

        var currentStep = GetCurrentStep(request.RequestFlowStpes, StepType.PINConfirmation);

        if (currentStep is null)
        {
            return null;
        }

        request.UpdateCurrentStep(currentStep.Type);

        var supportiveStep = request.RequestFlowStpes.Where(x => x.FlowStepType == FlowStepType.Supportive).ToList();

        var mainSteps = request.RequestFlowStpes.Except(supportiveStep).ToList();

        if (mainSteps.TrueForAll(x => x.FlowStepType == FlowStepType.Main && x.IsCompleted))
        {
            if (request.RequestType == RequestType.Registration)
            {
                CreateUser(request);
            }
            else
            {
                var user = await databaseService.Users.FirstOrDefaultAsync(x => x.ICNumber == request.ICNumber, cancellationToken);

                if (user is null)
                {
                    return null;
                }

                user.IsVerifyEmail = request.RequestFlowStpes.Find(x => x.Type == StepType.EmailConfrmation && x.IsCompleted) is not null;
                user.IsVerifyPhoneNumber = request.RequestFlowStpes.Find(x => x.Type == StepType.PhoneNumberConfrmation && x.IsCompleted) is not null;
                user.IsVerifyPolicy = request.RequestFlowStpes.Find(x => x.Type == StepType.PolicyConfirmation && x.IsCompleted) is not null;
                user.IsVerifyPIN = request.RequestFlowStpes.Find(x => x.Type == StepType.PINConfirmation && x.IsCompleted) is not null;
            }
        }

        request.UpdateNextStep(StepType.BiometricConfirmation);

        await databaseService.SaveChangesAsync(cancellationToken);

        return new CheckResponse()
        {
            NextStep = StepType.BiometricConfirmation.ToString(),
            RequestId = request.Id
        };
    }

    public async Task<CheckResponse> BiomtricCheck(Guid requestId, CancellationToken cancellationToken)
    {
        var request = await databaseService
            .Requests
            .Include(x => x.RequestFlowStpes)
            .FirstOrDefaultAsync(x => x.Id == requestId, cancellationToken);

        if (request is null)
        {
            return null;
        }

        var currentStep = GetCurrentStep(request.RequestFlowStpes, StepType.BiometricConfirmation);

        if (currentStep is null)
        {
            return null;
        }

        request.UpdateCurrentStep(currentStep.Type);

        if (request.RequestFlowStpes.TrueForAll(x => x.IsCompleted))
        {
            var user = await databaseService.Users.FirstOrDefaultAsync(x => x.ICNumber == request.ICNumber, cancellationToken);

            if (user is null)
            {
                if (CreateUser(request) is null)
                {
                    return null;
                }
            }
            else
            {
                user.IsVerifyBiometric = true;
            }
        }

        await databaseService.SaveChangesAsync(cancellationToken);

        return new CheckResponse()
        {
            NextStep = "Profile Verified",
            RequestId = request.Id
        };
    }

    private async Task<List<RequestFlowStpe>> GetSteps(CancellationToken cancellationToken)
    {
        var steps = await databaseService
            .Steps
            .ToListAsync(cancellationToken);

        return steps.ConvertAll(i => new RequestFlowStpe()
        {
            IsCompleted = false,
            Status = i.Type == StepType.PhoneNumberConfrmation ? FlowStepStatus.InProgress : FlowStepStatus.NotProcessed,
            Type = i.Type,
            IsFirstStep = i.Type == StepType.PhoneNumberConfrmation,
            IsLastStep = i.Type == StepType.BiometricConfirmation,
            FlowStepType = i.FlowStepType
        });
    }

    private User CreateUser(Request request)
    {
        if (UserCheck(request.Name, request.Email, request.ICNumber, request.PhoneNumber).Result)
        {
            return null;
        }

        var user = new User()
        {
            Name = request.Name,
            Email = request.Email,
            ICNumber = request.ICNumber,
            PhoneNumber = request.PhoneNumber,
            PIN = request.PIN,
            IsVerifyEmail = request.RequestFlowStpes.Find(x => x.Type == StepType.EmailConfrmation && x.IsCompleted) is not null,
            IsVerifyPhoneNumber = request.RequestFlowStpes.Find(x => x.Type == StepType.PhoneNumberConfrmation && x.IsCompleted) is not null,
            IsVerifyPolicy = request.RequestFlowStpes.Find(x => x.Type == StepType.PolicyConfirmation && x.IsCompleted) is not null,
            IsVerifyBiometric = request.RequestFlowStpes.Find(x => x.Type == StepType.BiometricConfirmation && x.IsCompleted) is not null,
            IsVerifyPIN = request.RequestFlowStpes.Find(x => x.Type == StepType.PINConfirmation && x.IsCompleted) is not null,
        };

        databaseService.Users.Add(user);

        return user;
    }

    private async Task<bool> UserCheck(string name, string email, string icNumber, string phoneNumber)
    {
        if (await databaseService.Users.FirstOrDefaultAsync(x => x.Name == name) is not null)
        {
            return true;
        }

        if (await databaseService.Users.FirstOrDefaultAsync(x => x.Email == email) is not null)
        {
            return true;
        }

        if (await databaseService.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber) is not null)
        {
            return true;
        }

        if (await databaseService.Users.FirstOrDefaultAsync(x => x.ICNumber == icNumber) is not null)
        {
            return true;
        }

        return false;
    }

    private static RequestFlowStpe GetCurrentStep(List<RequestFlowStpe> requestFlowStpes, StepType stepType)
    {
        return requestFlowStpes.Find(x => (x.Status == FlowStepStatus.InProgress || x.Status == FlowStepStatus.Failed) && x.Type == stepType);
    }

    private async Task<Request> CreateRequest(
        string name,
        string icNumber,
        string email,
        string phoneNumber,
        RequestType requestType,
        List<RequestFlowStpe> requestFlowStpes,
        CancellationToken cancellationToken)
    {
        var request = new Request()
        {
            Email = email,
            ICNumber = icNumber,
            Name = name,
            PhoneNumber = phoneNumber,
            RequestType = requestType,
            RequestFlowStpes = requestFlowStpes
        };

        databaseService.Requests.Add(request);

        await databaseService.SaveChangesAsync(cancellationToken);

        return request;
    }

    private async Task<List<RequestFlowStpe>> ProcessSteps(string icNumber, CancellationToken cancellationToken)
    {
        var request = await databaseService
            .Requests
            .Include(x => x.RequestFlowStpes)
            .FirstOrDefaultAsync(x => x.ICNumber == icNumber, cancellationToken);

        if (request is null)
        {
            return null;
        }

        var steps = new List<RequestFlowStpe>();

        if (request.RequestFlowStpes.Exists(x => !x.IsCompleted && x.FlowStepType != FlowStepType.Main))
        {
            steps.Add(request.RequestFlowStpes.Find(x => !x.IsCompleted));
        }

        return steps;
    }
}