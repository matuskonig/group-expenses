using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WebApplication.Models
{
    /// <summary>
    /// Class representing user, which is created in registration process
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<FriendshipStatus> IncomingRequests { get; set; }
        public virtual ICollection<FriendshipStatus> SentRequests { get; set; }

        /// <summary>
        ///  All payment groups, in which the user is part of
        /// </summary>
        public virtual ICollection<SinglePurposeUserGroup> PaymentGroups { get; set; }
    }
}