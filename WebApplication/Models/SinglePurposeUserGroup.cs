using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    /// <summary>
    /// Group, which is representing single event, in which multiple payments are present
    /// </summary>
    public class SinglePurposeUserGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Users, which are part of the group
        /// </summary>
        public virtual ICollection<ApplicationUser> GroupUsers { get; set; }

        /// <summary>
        /// All group payments, containing the name, user sending money and receivers
        /// </summary>
        public virtual ICollection<UnidirectionalPaymentGroup> GroupPayments { get; set; }
    }
}