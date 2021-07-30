using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Entities.AuthDto;
using Entities.UserDto;

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

        public async void SendNewFriendRequest(UserDto user)
        {
            var url = $"user/sendNewRequest/{user.Id}";
            var result = await httpClient.GetAsync(url);
            if (result.StatusCode != HttpStatusCode.OK)
                throw new ArgumentException("wrong user");
        }

        public async void AcceptFriendRequest(FriendRequestDto request)
        {
            var response = await httpClient.GetAsync($"user/acceptRequest/{request.Id}");
            if (!response.IsSuccessStatusCode)
                throw new ArgumentException("bad sth");
            var friendRequest = await response.Content.ReadFromJsonAsync<FriendRequestDto>();
            UpdateIncomingFriendRequest(friendRequest);
        }

        public async void RejectFriendRequest(FriendRequestDto request)
        {
            var response = await httpClient.GetAsync($"user/rejectRequest/{request.Id}");
            if (!response.IsSuccessStatusCode)
                throw new ArgumentException("bad sth");
            var friendRequest = await response.Content.ReadFromJsonAsync<FriendRequestDto>();
            UpdateIncomingFriendRequest(friendRequest);
        }

        private void UpdateIncomingFriendRequest(FriendRequestDto friendRequest)
        {
            CurrentUser.IncomingRequests = CurrentUser.IncomingRequests.Select(incomingRequest =>
                incomingRequest.Id == friendRequest?.Id
                    ? friendRequest
                    : incomingRequest);
            OnChange?.Invoke();
        }

        public async Task<SearchUserResponse> SearchUsers(SearchUserRequest request)
        {
            var response = await httpClient.PostAsJsonAsync("user/searchUser", request);
            var data = await response.Content.ReadFromJsonAsync<SearchUserResponse>();
            return data;
        }

        public async Task<IEnumerable<UserDto>> SearchFriendByName(string username)
        {
            var data = await SearchUsers(new SearchUserRequest {UserName = username});
            return data.UsersByUserName ;
        }
    }
}