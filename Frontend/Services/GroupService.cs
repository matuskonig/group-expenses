using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Entities.Dto.GroupDto;
using Frontend.Helpers;

namespace Frontend.Services
{
    public class GroupService
    {
        private readonly HttpClient _httpClient;
        private IEnumerable<SinglePurposeUserGroupDto> _userGroups = Enumerable.Empty<SinglePurposeUserGroupDto>();
        public event Action OnChange;

        public GroupService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IEnumerable<SinglePurposeUserGroupDto> UserGroups
        {
            get => _userGroups;
            set
            {
                _userGroups = value;
                OnChange?.Invoke();
            }
        }

        public async Task LoadAll()
        {
            var data =
                await _httpClient.GetFromJsonAsync<IEnumerable<SinglePurposeUserGroupDto>>("group/currentUserGroup");
            if (data != null)
            {
                UserGroups = data;
            }
        }

        public async Task<SinglePurposeUserGroupDto> AddNewGroup(string groupName)
        {
            var response = await _httpClient.PostAsJsonAsync("group/addNewGroup",
                new SinglePurposeUserGroupDto { Name = groupName });
            if (!response.IsSuccessStatusCode)
                throw new Exception("sth went wrong");
            var data = await response.Content.ReadFromJsonAsync<SinglePurposeUserGroupDto>();
            return data;
        }

        public async Task<SinglePurposeUserGroupDto> ModifyUserGroup(SinglePurposeUserGroupDto group)
        {
            var response = await _httpClient.PatchAsJsonAsync("group/modifyUserGroup", group);
            if (!response.IsSuccessStatusCode)
                throw new Exception("sth went wrong");
            var data = await response.Content.ReadFromJsonAsync<SinglePurposeUserGroupDto>();
            return data;
        }

        public async Task<UnidirectionalPaymentGroupDto> ModifyPaymentGroup(UnidirectionalPaymentGroupDto paymentGroup)
        {
            var response = await _httpClient.PatchAsJsonAsync("group/modifyPaymentGroup", paymentGroup);
            if (!response.IsSuccessStatusCode)
                throw new Exception("sth went wrong");
            var data = await response.Content.ReadFromJsonAsync<UnidirectionalPaymentGroupDto>();
            return data;
        }

        public async Task<SinglePaymentDto> ModifySinglePayment(SinglePaymentDto payment)
        {
            var response = await _httpClient.PatchAsJsonAsync("group/modifySinglePayment", payment);
            if (!response.IsSuccessStatusCode)
                throw new Exception("something is wrong");
            var data = await response.Content.ReadFromJsonAsync<SinglePaymentDto>();
            return data;
        }

        public async Task<IEnumerable<PaymentRecordDto>> GetGroupSettlement(SinglePurposeUserGroupDto group)
        {
            var response = await _httpClient.GetAsync($"group/getGroupSettlement/{group.Id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("sth went wrong");
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<PaymentRecordDto>>();
            return data;
        }
    }
}