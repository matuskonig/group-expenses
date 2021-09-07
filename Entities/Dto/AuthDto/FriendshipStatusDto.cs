using System;
using Entities.Enums;

namespace Entities.Dto.AuthDto
{
    public class FriendshipStatusDto
    {
        public Guid Id { get; set; }
        public UserDto From { get; set; }
        public UserDto To { get; set; }
        public DateTime Created { get; set; }
        public FriendRequestState State { get; set; }
        public DateTime Modified { get; set; }
    }
}