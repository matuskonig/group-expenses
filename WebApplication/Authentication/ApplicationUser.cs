using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using WebApplication.Models;

namespace WebApplication.Authentication
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<FriendshipStatus> IncomingRequests { get; set; }
        public virtual ICollection<FriendshipStatus> SentRequests { get; set; }
        public virtual ICollection<SinglePurposeUserGroup> PaymentGroups { get; set; }
    }
}