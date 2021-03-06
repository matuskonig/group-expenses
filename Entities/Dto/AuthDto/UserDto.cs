using System.Collections.Generic;

namespace Entities.Dto.AuthDto
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public IEnumerable<FriendshipStatusDto> IncomingRequests { get; set; }
        public IEnumerable<FriendshipStatusDto> SentRequests { get; set; }
    }
}