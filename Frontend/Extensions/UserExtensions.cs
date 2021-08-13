using System.Collections.Generic;
using System.Linq;
using Entities.AuthDto;
using Entities.Enums;

namespace Frontend.Extensions
{
    public static class UserExtensions
    {
        public static IEnumerable<UserDto> Friends(this UserDto currentUser)
        {
            var incoming = currentUser.IncomingRequests?
                .Where(request => request.State == FriendRequestState.Accepted)
                .Select(request => request.From) ?? Enumerable.Empty<UserDto>();
            var sent = currentUser.SentRequests?
                .Where(request => request.State == FriendRequestState.Accepted)
                .Select(request => request.To) ?? Enumerable.Empty<UserDto>();
            return incoming.Concat(sent);
        }
    }
}