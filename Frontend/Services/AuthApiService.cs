using System;
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

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var response = await httpClient.PostAsJsonAsync("auth/login", request);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Login failed");
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {result.Token}");
            return result;
        }

        public async Task Register(RegisterRequest request)
        {
            var response = await httpClient.PostAsJsonAsync("auth/register", request);
            if (!response.IsSuccessStatusCode)
                throw new ArgumentException("something failed");
        }

        public void Logout()
        {
            httpClient.DefaultRequestHeaders.Remove("Authorization");
        }
    }
}