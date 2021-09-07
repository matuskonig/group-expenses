using WebApplication.Models;

namespace WebApplication.Algorithms.Group
{
    /// <summary>
    /// Record, containing info about single payment (single edge in payment graph)
    /// </summary>
    public record SinglePaymentRecord
    {
        public ApplicationUser PaymentBy { get; init; }
        public ApplicationUser PaymentFor { get; init; }
        public decimal Price { get; init; }
    }
}