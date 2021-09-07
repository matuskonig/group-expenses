using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Enums;

namespace WebApplication.Models
{
    /// <summary>
    /// Class representing the friendship between 2 users
    /// </summary>
    public sealed class FriendshipStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        // Defining properties of oriented edge
        // Users are friends, if there exist a relation (user sent request or received request) and the relation is
        // accepted
        [Required] public ApplicationUser From { get; set; }
        [Required] public ApplicationUser To { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public FriendRequestState State { get; set; }
    }
}