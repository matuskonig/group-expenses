using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Entities.Dto.GroupDto;
using Frontend.Extensions;
using Frontend.Helpers;

namespace Frontend.Services
{
    /// <summary>
    /// Service used to manage groups, which the user is part of
    /// </summary>
    public class GroupService
    {
        private readonly HttpClient _httpClient;
        private readonly AlertMessageService _alertMessageService;
        private IEnumerable<SinglePurposeUserGroupDto> _userGroups = Enumerable.Empty<SinglePurposeUserGroupDto>();
        public event Action OnChange;

        public GroupService(HttpClient httpClient, AlertMessageService alertMessageService)
        {
            _httpClient = httpClient;
            _alertMessageService = alertMessageService;
        }

        /// <summary>
        /// SinglePurposeUserGroups, in which the current user is member of
        /// </summary>
        public IEnumerable<SinglePurposeUserGroupDto> UserGroups
        {
            get => _userGroups;
            private set
            {
                _userGroups = value;
                OnChange?.Invoke();
            }
        }

        /// <summary>
        /// Loads all current user groups, if not successful, shows error
        /// </summary>
        public async Task LoadAll()
        {
            var response = await _httpClient.GetAsync("group/currentUserGroup");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<IEnumerable<SinglePurposeUserGroupDto>>();
                if (data != null)
                {
                    UserGroups = data;
                }
            }
            else
            {
                await _alertMessageService.ShowNetworkError(response);
            }
        }

        /// <summary>
        /// Creates a new group, in which the current user is only member
        /// </summary>
        /// <param name="groupName">Name of the newly created user group</param>
        /// <returns>Newly created group</returns>
        public async Task<SinglePurposeUserGroupDto> AddNewGroup(string groupName)
        {
            var response = await _httpClient.PostAsJsonAsync("group/addNewGroup",
                new SinglePurposeUserGroupDto { Name = groupName });
            if (!response.IsSuccessStatusCode)
            {
                await _alertMessageService.ShowNetworkError(response);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<SinglePurposeUserGroupDto>();
        }

        /// <summary>
        /// Used to modify either scalar values of the group, or to adding or removing collection entities
        /// </summary>
        /// <param name="group">modified group</param>
        /// <returns>group with modified data</returns>
        public async Task<SinglePurposeUserGroupDto> ModifyUserGroup(SinglePurposeUserGroupDto group)
        {
            var response = await _httpClient.PatchAsJsonAsync("group/modifyUserGroup", group);
            if (!response.IsSuccessStatusCode)
            {
                await _alertMessageService.ShowNetworkError(response);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<SinglePurposeUserGroupDto>();
        }

        /// <summary>
        /// Modifies the payment group, either by directly modifying the scalar values or by adding/removing entities
        /// from collections
        /// </summary>
        /// <param name="paymentGroup">Payment group to modify</param>
        /// <returns>modified payment group</returns>
        public async Task<UnidirectionalPaymentGroupDto> ModifyPaymentGroup(UnidirectionalPaymentGroupDto paymentGroup)
        {
            var response = await _httpClient.PatchAsJsonAsync("group/modifyPaymentGroup", paymentGroup);
            if (!response.IsSuccessStatusCode)
            {
                await _alertMessageService.ShowNetworkError(response);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<UnidirectionalPaymentGroupDto>();
        }

        /// <summary>
        /// Modify single payment
        /// </summary>
        /// <param name="payment">single payment to modify</param>
        /// <returns>modified payment</returns>
        public async Task<SinglePaymentDto> ModifySinglePayment(SinglePaymentDto payment)
        {
            var response = await _httpClient.PatchAsJsonAsync("group/modifySinglePayment", payment);
            if (!response.IsSuccessStatusCode)
            {
                await _alertMessageService.ShowNetworkError(response);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<SinglePaymentDto>();
        }

        /// <summary>
        /// Gets group settlement, the list of payments, which if executed, group will be even and no member will owe
        /// money to anyone
        /// </summary>
        /// <param name="group">group to find the settlement from</param>
        /// <returns>list of payments required</returns>
        public async Task<IEnumerable<PaymentRecordDto>> GetGroupSettlement(SinglePurposeUserGroupDto group)
        {
            var response = await _httpClient.GetAsync($"group/getGroupSettlement/{group.Id}");
            if (!response.IsSuccessStatusCode)
            {
                await _alertMessageService.ShowNetworkError(response);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<IEnumerable<PaymentRecordDto>>();
        }
    }
}