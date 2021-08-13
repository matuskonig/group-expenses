using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication.Authentication;

namespace WebApplication.Models
{
    public class UnidirectionalPaymentGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public ApplicationUser PaymentBy { get; set; }
        public virtual ICollection<SinglePayment> PaymentTargets { get; set; }
    }
}