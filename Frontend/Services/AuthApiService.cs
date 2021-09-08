using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Entities.Dto.AuthDto;
using Frontend.Extensions;

namespace Frontend.Services
{
    /// <summary>
    /// Service used to manage user login status, connected to CascadingValue
    /// </summary>
    public class AuthApiService
    {
        public event Action OnChange;

        private readonly HttpClient _httpClient;
        private readonly AlertMessageService _alertMessageService;
        
        private JwtSecurityToken _jwtSecurityToken;

        /// <summary>
        /// Token received from BE, if not null, user has succeeded with login
        /// In any change in token, OnChanged handler is called
        /// </summary>
        public JwtSecurityToken JwtSecurityToken
        {
            get => _jwtSecurityToken;
            private set
            {
                _jwtSecurityToken = value;
                OnChange?.Invoke();
            }
        }

        public AuthApiService(HttpClient httpClient, AlertMessageService alertMessageService)
        {
            _httpClient = httpClient;
            _alertMessageService = alertMessageService;
        }

        public async Task<bool> Login(LoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", request);

            if (!response.IsSuccessStatusCode)
            {
                await _alertMessageService.ShowNetworkError(response);
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            JwtSecurityToken = new JwtSecurityToken(result.Token);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {result.Token}");
            return true;
        }

        public async Task Register(RegisterRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/register", request);
            if (!response.IsSuccessStatusCode)
                await _alertMessageService.ShowNetworkError(response);
        }

        public void Logout()
        {
            JwtSecurityToken = null;
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
        }
    }
}