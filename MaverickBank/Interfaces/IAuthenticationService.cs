using MaverickBank.Models.DTOs;

namespace MaverickBank.Interfaces
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> Login(UserLoginRequest loginRequest);
    }
}
