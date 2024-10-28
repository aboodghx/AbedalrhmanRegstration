namespace API.Inputs;

public class PINCheckInput : BaseInput
{
    public string PIN { get; set; }
    public string ConfirmPIN { get; set; }
}