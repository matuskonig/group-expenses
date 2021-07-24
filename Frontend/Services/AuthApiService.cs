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
        private readonly HttpClient httpClient;

        public AuthApiService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<JwtSecurityToken> Login(string email, string password)
        {
            var data = new LoginRequest {Email = email, Password = password};
            var response = await httpClient.PostAsJsonAsync("auth/login", data);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Login failed");
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {result.Token}");
            return new JwtSecurityToken(result.Token);
        }

        public async Task Register(string email, string username, string password)
        {
            var data = new RegisterRequest {Email = email, Username = username, Password = password};
            var response = await httpClient.PostAsJsonAsync("auth/register", data);
            if (!response.IsSuccessStatusCode)
                throw new ArgumentException("something failed");
        }

        public void Logout()
        {
            httpClient.DefaultRequestHeaders.Remove("Authorization");
        }
    }
}