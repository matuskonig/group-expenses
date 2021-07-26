using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using WebApplication.Models;

namespace WebApplication.Authentication
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<FriendRequest> IncomingRequests { get; set; }
        public ICollection<FriendRequest> SentRequests { get; set; }
    }
}