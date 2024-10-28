using Identity.Models.Enums;

namespace Identity.Models;

public class RequestFlowStpe
{
    public Guid Id { get; set; }

    public Guid RequestId { get; set; }
    public Request Request { get; set; }

    public StepType Type { get; set; }
    public FlowStepType FlowStepType { get; set; }
    public bool IsFirstStep { get; set; }
    public bool IsLastStep { get; set; }
    public bool IsCompleted { get; set; }
    public FlowStepStatus Status { get; set; }

    public void CompleteStep()
    {
        if (Status == FlowStepStatus.InProgress)
        {
            Status = FlowStepStatus.Done;
            IsCompleted = true;
        }
    }
}