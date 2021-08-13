using System.Collections.Generic;

namespace Entities.AuthDto
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public IEnumerable<FriendRequestDto> IncomingRequests { get; set; }
        public IEnumerable<FriendRequestDto> SentRequests { get; set; }
    }
}