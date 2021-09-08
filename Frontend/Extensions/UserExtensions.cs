using System.Collections.Generic;
using System.Linq;
using Entities.Dto.AuthDto;
using Entities.Enums;

namespace Frontend.Extensions
{
    public static class UserExtensions
    {
        /// <summary>
        /// Returns all friends of the user - user, who sent request to the user and users, to which current user sent request
        /// and the state is accepted
        /// </summary>
        /// <param name="currentUser">Current user</param>
        /// <returns>List of friends of the user</returns>
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