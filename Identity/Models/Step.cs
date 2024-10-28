using Identity.Models.Enums;

namespace Identity.Models;

public class Step
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public StepType Type { get; set; }
    public FlowStepType FlowStepType { get; set; }
}