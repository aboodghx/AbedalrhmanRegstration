using App.Models;

namespace App.Services;

public interface IUserService
{
    Task<CheckResponse> BiomtricCheck(Guid requestId, CancellationToken cancellationToken);

    Task<CheckResponse> CreateRequest(string name, string email, string icNumber, string phoneNumber, CancellationToken cancellationToken);

    Task<CheckResponse> EmailCheck(Guid requestId, string code, CancellationToken cancellationToken);
    Task<CheckResponse> Login(string icNumber, CancellationToken cancellationToken);
    Task<CheckResponse> PhoneNumberCheck(Guid requestId, string code, CancellationToken cancellationToken);

    Task<CheckResponse> PINCheck(Guid requestId, string pin, CancellationToken cancellationToken);

    Task<CheckResponse> PolicyCheck(Guid requestId, CancellationToken cancellationToken);
}