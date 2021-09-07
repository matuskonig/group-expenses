using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    /// <summary>
    /// Group, representing single named payment of one person for multiple persons
    /// </summary>
    public class UnidirectionalPaymentGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        
        /// <summary>
        /// User who paid the payment
        /// </summary>
        public ApplicationUser PaymentBy { get; set; }
        
        /// <summary>
        /// Receivers of the payment
        /// </summary>
        public virtual ICollection<SinglePayment> PaymentTargets { get; set; }
        
        /// <summary>
        /// Navigation property
        /// </summary>
        public virtual SinglePurposeUserGroup UserGroup { get; set; }
    }
}