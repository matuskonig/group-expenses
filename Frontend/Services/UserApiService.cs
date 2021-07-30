using System;
using System.Net.Http;
using System.Net.Http.Json;
using Entities.AuthDto;

namespace Frontend.Services
{
    public class UserApiService
    {
        private readonly HttpClient httpClient;
        
        
        private UserDto _currentUser;
        public UserDto CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnChange?.Invoke();
            }
        }
        public event Action OnChange;

        public UserApiService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async void LoadCurrent()
        {
            var result = await httpClient.GetFromJsonAsync<UserDto>("user/current");
            CurrentUser ??= result;
        }
        public void SendNewFriendRequest(UserDto id)
        {
            
        }
        public void AcceptFriendRequest(UserDto user){}

        public void RejectFriendRequest(UserDto user)
        {
            
        }

        public void SearchFriends()
        {
            
        }

        public void SearchFriendByName(string username)
        {
            
        }
        
    }
}