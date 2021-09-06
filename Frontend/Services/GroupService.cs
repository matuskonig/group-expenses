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
    public class GroupService
    {
        private readonly HttpClient _httpClient;
        private AlertMessageService _alertMessageService;
        private IEnumerable<SinglePurposeUserGroupDto> _userGroups = Enumerable.Empty<SinglePurposeUserGroupDto>();
        public event Action OnChange;

        public GroupService(HttpClient httpClient, AlertMessageService alertMessageService)
        {
            _httpClient = httpClient;
            _alertMessageService = alertMessageService;
        }

        public IEnumerable<SinglePurposeUserGroupDto> UserGroups
        {
            get => _userGroups;
            private set
            {
                _userGroups = value;
                OnChange?.Invoke();
            }
        }

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