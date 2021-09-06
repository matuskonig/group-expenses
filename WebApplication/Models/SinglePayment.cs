using System;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication.Authentication;

namespace WebApplication.Models
{
    public sealed class SinglePayment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public ApplicationUser Target { get; set; }
        public UnidirectionalPaymentGroup PaymentGroup { get; set; }
    }
}