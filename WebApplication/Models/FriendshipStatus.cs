using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Enums;
using WebApplication.Authentication;

namespace WebApplication.Models
{
    public sealed class FriendshipStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public ApplicationUser From { get; set; }
        public ApplicationUser To { get; set; }
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Created { get; set; }
        
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Modified { get; set; }
        public FriendRequestState State { get; set; }
    }
}