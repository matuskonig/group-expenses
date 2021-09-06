using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Entities.Dto.AuthDto;
using Entities.Dto.UserDto;
using Frontend.Extensions;

namespace Frontend.Services
{
    public class UserApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AlertMessageService _alertMessageService;

        private UserDto _currentUser;

        public UserDto CurrentUser
        {
            get => _currentUser;
            private set
            {
                _currentUser = value;
                OnChange?.Invoke();
            }
        }

        public event Action OnChange;

        public UserApiService(HttpClient httpClient, AlertMessageService alertMessageService)
        {
            _httpClient = httpClient;
            _alertMessageService = alertMessageService;
        }

        public async Task LoadCurrent()
        {
            var response = await _httpClient.GetAsync("user/current");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<UserDto>();
                CurrentUser = data;
            }
            else
            {
                await _alertMessageService.ShowNetworkError(response);
            }
        }

        public async Task SendNewFriendRequest(UserDto user)
        {
            var response = await _httpClient.GetAsync($"user/sendNewRequest/{user.Id}");
            if (!response.IsSuccessStatusCode)
                await _alertMessageService.ShowNetworkError(response);
        }

        public async Task AcceptFriendRequest(FriendRequestDto request)
        {
            var response = await _httpClient.GetAsync($"user/acceptRequest/{request.Id}");
            if (!response.IsSuccessStatusCode)
            {
                await _alertMessageService.ShowNetworkError(response);
                return;
            }

            var friendRequest = await response.Content.ReadFromJsonAsync<FriendRequestDto>();
            UpdateIncomingFriendRequest(friendRequest);
            UpdateSendFriendRequest(friendRequest);
            OnChange?.Invoke();
        }

        public async Task RejectFriendRequest(FriendRequestDto request)
        {
            var response = await _httpClient.GetAsync($"user/rejectRequest/{request.Id}");
            if (!response.IsSuccessStatusCode)
            {
                await _alertMessageService.ShowNetworkError(response);
                return;
            }

            var friendRequest = await response.Content.ReadFromJsonAsync<FriendRequestDto>();
            UpdateIncomingFriendRequest(friendRequest);
            UpdateSendFriendRequest(friendRequest);
            OnChange?.Invoke();
        }

        private void UpdateIncomingFriendRequest(FriendRequestDto updatedFriendRequest)
        {
            CurrentUser.IncomingRequests = CurrentUser.IncomingRequests
                .Select(incomingRequest =>
                    incomingRequest.Id == updatedFriendRequest?.Id
                        ? updatedFriendRequest
                        : incomingRequest)
                .ToArray();
        }

        private void UpdateSendFriendRequest(FriendRequestDto updatedFriendRequest)
        {
            CurrentUser.SentRequests = CurrentUser.SentRequests
                .Select(sentRequest => sentRequest.Id == updatedFriendRequest?.Id
                    ? updatedFriendRequest
                    : sentRequest)
                .ToArray();
        }

        private async Task<SearchUserResponse> SearchUsers(SearchUserRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("user/searchUser", request);
            if (!response.IsSuccessStatusCode)
            {
                await _alertMessageService.ShowNetworkError(response);
                return null;
            }

            var data = await response.Content.ReadFromJsonAsync<SearchUserResponse>();
            return data;
        }

        public async Task<IEnumerable<UserDto>> SearchFriendByName(string username)
        {
            var data = await SearchUsers(new SearchUserRequest { UserName = username });
            return data?.UsersByUserName;
        }
    }
}