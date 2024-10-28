namespace Identity.Models;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ICNumber { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string PIN { get; set; }
    public bool IsVerifyPhoneNumber { get; set; }
    public bool IsVerifyPIN { get; set; }
    public bool IsVerifyEmail { get; set; }
    public bool IsVerifyPolicy { get; set; }
    public bool IsVerifyBiometric { get; set; }
}