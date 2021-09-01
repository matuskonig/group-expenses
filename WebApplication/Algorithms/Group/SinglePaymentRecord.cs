using WebApplication.Authentication;

namespace WebApplication.Algorithms.Group
{
    public record SinglePaymentRecord
    {
        public ApplicationUser PaymentBy { get; init; }
        public ApplicationUser PaymentFor { get; init; }
        public decimal Price { get; init; }
    }
}