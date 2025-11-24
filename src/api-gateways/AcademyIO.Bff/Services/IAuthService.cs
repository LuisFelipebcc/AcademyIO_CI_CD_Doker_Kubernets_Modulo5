using AcademyIO.Bff.Models;

namespace AcademyIO.Bff.Services
{
    public interface IAuthService
    {
        Task<UserLoginResponse> Login(UserLogin login);
        Task<UserLoginResponse> Register(UserRegister register);
    }
}
