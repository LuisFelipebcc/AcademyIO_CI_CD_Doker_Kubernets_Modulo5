using AcademyIO.Bff.Extensions;
using AcademyIO.Bff.Models;
using Microsoft.Extensions.Options;

namespace AcademyIO.Bff.Services
{
    public class AuthService : Service, IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.AuthUrl);
        }

        public async Task<UserLoginResponse> Login(UserLogin login)
        {
            var loginContent = GetContent(login);
            var response = await _httpClient.PostAsync("/api/auth/auth", loginContent);

            if (!ManageHttpResponse(response))
            {
                return new UserLoginResponse
                {
                    ResponseResult = await DeserializeResponse<Core.Communication.ResponseResult>(response)
                };
            }

            return await DeserializeResponse<UserLoginResponse>(response);
        }

        public async Task<UserLoginResponse> Register(UserRegister register)
        {
            var registerContent = GetContent(register);
            var response = await _httpClient.PostAsync("/api/auth/new-account", registerContent);

            if (!ManageHttpResponse(response))
            {
                return new UserLoginResponse
                {
                    ResponseResult = await DeserializeResponse<Core.Communication.ResponseResult>(response)
                };
            }

            return await DeserializeResponse<UserLoginResponse>(response);
        }
    }
}
