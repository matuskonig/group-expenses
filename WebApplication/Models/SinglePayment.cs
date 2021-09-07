using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class SinglePayment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public ApplicationUser Target { get; set; }
        
        /// <summary>
        /// Navigation property to owning UnidirectionalPaymentGroup
        /// </summary>
        public virtual UnidirectionalPaymentGroup PaymentGroup { get; set; }
    }
}