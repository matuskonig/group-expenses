using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Entities.AuthDto;

namespace Frontend.Services
{
    public class AuthApiService
    {
        public AuthApiService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task Login(LoginRequest request)
        {
            var response = await httpClient.PostAsJsonAsync("auth/login", request);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Login failed");
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>() ??
                         throw new ArgumentException("result cannot be null");
            JwtSecurityToken = new JwtSecurityToken(result.Token);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {result.Token}");
        }

        public async Task Register(RegisterRequest request)
        {
            var response = await httpClient.PostAsJsonAsync("auth/register", request);
            if (!response.IsSuccessStatusCode)
                throw new ArgumentException("Registration failed");
        }

        public void Logout()
        {
            JwtSecurityToken = null;
            httpClient.DefaultRequestHeaders.Remove("Authorization");
        }

        private JwtSecurityToken _jwtSecurityToken;

        public JwtSecurityToken JwtSecurityToken
        {
            get => _jwtSecurityToken;
            set
            {
                _jwtSecurityToken = value;
                OnChange?.Invoke();
            }
        }

        public event Action OnChange;

        private readonly HttpClient httpClient;
    }
}