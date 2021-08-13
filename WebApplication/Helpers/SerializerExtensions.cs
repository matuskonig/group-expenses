using System.Linq;
using Entities.AuthDto;
using WebApplication.Authentication;
using WebApplication.Models;

namespace WebApplication.Helpers
{
    public static class SerializerExtensions
    {
        public static UserDto Serialize(this ApplicationUser user, bool serializeRequests = true)
        {
            return new()
            {
                Id = user.Id,
                UserName = user.UserName,
                IncomingRequests = serializeRequests ? user.IncomingRequests?.Select(Serialize): null,
                SentRequests = serializeRequests ? user.SentRequests?.Select(Serialize) : null
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
                From = friendshipStatus.From?.Serialize(serializeRequests: false),
                To = friendshipStatus.To?.Serialize(serializeRequests: false)
            };
        }
    }
}