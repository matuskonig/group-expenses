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

        [Required] public ApplicationUser From { get; set; }
        [Required] public ApplicationUser To { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public FriendRequestState State { get; set; }
    }
}