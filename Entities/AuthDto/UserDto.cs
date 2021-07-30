using System.Collections.Generic;
using System.Linq;
using Entities.Enums;

namespace Entities.AuthDto
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public IEnumerable<FriendRequestDto> IncomingRequests { get; set; }
        public IEnumerable<FriendRequestDto> SentRequests { get; set; }

        public IEnumerable<UserDto> Friends
        {
            get
            {
                var incoming = IncomingRequests?
                    .Where(request => request.State == FriendRequestState.Accepted)
                    .Select(request => request.From) ?? Enumerable.Empty<UserDto>();
                var sent = SentRequests?
                    .Where(request => request.State == FriendRequestState.Accepted)
                    .Select(request => request.To) ?? Enumerable.Empty<UserDto>();
                ;
                return incoming.Concat(sent);
            }
        }
    }
}