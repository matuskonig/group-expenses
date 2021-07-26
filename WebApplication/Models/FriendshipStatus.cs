using System;
using Entities.Enums;
using WebApplication.Authentication;

namespace WebApplication.Models
{
    public class FriendRequest
    {
        public Guid Id { get; set; }
        public ApplicationUser From { get; set; }
        public ApplicationUser To { get; set; }
        public DateTime Created { get; set; }
        public FriendRequestState State { get; set; }
    }
}