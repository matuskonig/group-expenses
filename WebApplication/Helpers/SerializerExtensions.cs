using System.Linq;
using Entities.AuthDto;
using WebApplication.Authentication;
using WebApplication.Models;

namespace WebApplication.Helpers
{
    public static class SerializerExtensions
    {
        public static UserDto Serialize(this ApplicationUser user)
        {
            return new()
            {
                Id = user.Id,
                UserName = user.UserName,
                IncomingRequests = user.IncomingRequests?.Select(Serialize),
                SentRequests = user.SentRequests?.Select(Serialize)
            };
        }

        public static FriendRequestDto Serialize(this FriendshipStatus friendshipStatus)
        {
            return new()
            {
                Id = friendshipStatus.Id,
                Created = friendshipStatus.Created,
                Modified = friendshipStatus.Modified,
                State = friendshipStatus.State,
                From = friendshipStatus.From?.Serialize(),
                To = friendshipStatus.To?.Serialize()
            };
        }
    }
}