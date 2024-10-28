using Identity.Models.Enums;

namespace Identity.Models;

public class Request
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ICNumber { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string PIN { get; set; }
    public RequestType RequestType { get; set; }

    public List<RequestFlowStpe> RequestFlowStpes { get; set; }

    public void UpdateCurrentStep(StepType type)
    {
        var currentStep = RequestFlowStpes.Find(x => x.Type == type);

        currentStep.Status = FlowStepStatus.Done;
        currentStep.IsCompleted = true;
    }

    public void UpdateNextStep(StepType type)
    {
        var nextStep = RequestFlowStpes.Find(x => x.Type == type);

        nextStep.Status = FlowStepStatus.InProgress;
    }
}