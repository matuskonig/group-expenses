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
    /// <summary>
    /// Service used to manage current user and his properties
    /// </summary>
    public class UserApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AlertMessageService _alertMessageService;

        private UserDto _currentUser;

        /// <summary>
        /// Current user data, on any change done OnChange handler is caller
        /// </summary>
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

        /// <summary>
        /// Loads the current user and stores it in the variable.
        /// Note that OnChange handler is called
        /// </summary>
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

        /// <summary>
        /// Sends a friend request to user specified. If the operation fail in the BE, error is shown
        /// </summary>
        /// <param name="user">User to send the request to</param>
        public async Task SendNewFriendRequest(UserDto user)
        {
            var response = await _httpClient.GetAsync($"user/sendNewRequest/{user.Id}");
            if (!response.IsSuccessStatusCode)
                await _alertMessageService.ShowNetworkError(response);
        }

        /// <summary>
        /// Accepts the friend request. If the operation fails(request is already accepted) the message is shown
        /// </summary>
        /// <param name="request">Request to accept</param>
        public async Task AcceptFriendRequest(FriendshipStatusDto request)
        {
            var response = await _httpClient.GetAsync($"user/acceptRequest/{request.Id}");
            if (!response.IsSuccessStatusCode)
            {
                await _alertMessageService.ShowNetworkError(response);
                return;
            }

            var friendshipStatus = await response.Content.ReadFromJsonAsync<FriendshipStatusDto>();
            UpdateIncomingFriendRequest(friendshipStatus);
            UpdateSendFriendRequest(friendshipStatus);
            OnChange?.Invoke();
        }

        /// <summary>
        /// Puts the request in rejection state, cancelling the request if it was not accepted or cancelling the friendship
        /// if it was accepted
        /// </summary>
        /// <param name="request">Friendship request to reject</param>
        public async Task RejectFriendRequest(FriendshipStatusDto request)
        {
            var response = await _httpClient.GetAsync($"user/rejectRequest/{request.Id}");
            if (!response.IsSuccessStatusCode)
            {
                await _alertMessageService.ShowNetworkError(response);
                return;
            }

            var friendshipStatus = await response.Content.ReadFromJsonAsync<FriendshipStatusDto>();
            UpdateIncomingFriendRequest(friendshipStatus);
            UpdateSendFriendRequest(friendshipStatus);
            OnChange?.Invoke();
        }

        /// <summary>
        /// Replaces the modified friendship status with the new instance
        /// </summary>
        /// <param name="updatedFriendshipStatus">FriendshipStatus, in which some changes were made</param>
        private void UpdateIncomingFriendRequest(FriendshipStatusDto updatedFriendshipStatus)
        {
            CurrentUser.IncomingRequests = CurrentUser.IncomingRequests
                .Select(friendshipStatus =>
                    friendshipStatus.Id == updatedFriendshipStatus?.Id
                        ? updatedFriendshipStatus
                        : friendshipStatus)
                .ToList();
        }

        /// <summary>
        /// Replaces the modified friendship status with the new instance
        /// </summary>
        /// <param name="updatedFriendshipStatus">FriendshipStatus, in which some changes were made</param>
        private void UpdateSendFriendRequest(FriendshipStatusDto updatedFriendshipStatus)
        {
            CurrentUser.SentRequests = CurrentUser.SentRequests
                .Select(friendshipStatus => friendshipStatus.Id == updatedFriendshipStatus?.Id
                    ? updatedFriendshipStatus
                    : friendshipStatus)
                .ToList();
        }

        /// <summary>
        /// Given the search request, users are searched and eventually found users are returned according to categories
        /// </summary>
        /// <param name="request">Search request, notnull search category data means the search will be performed</param>
        /// <returns>Found user according to search categories</returns>
        private async Task<SearchUserResponse> SearchUsers(SearchUserRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("user/searchUser", request);
            if (!response.IsSuccessStatusCode)
            {
                await _alertMessageService.ShowNetworkError(response);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<SearchUserResponse>();
        }

        /// <summary>
        /// Searches the user according to their username
        /// </summary>
        /// <param name="username">part of the username</param>
        /// <returns>list of users, whose username contains username provided</returns>
        public async Task<IEnumerable<UserDto>> SearchFriendByName(string username)
        {
            var response = await SearchUsers(new SearchUserRequest { UserName = username });
            return response?.UsersByUserName;
        }
    }
}