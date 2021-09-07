using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication.Authentication;
using WebApplication.Models;

namespace WebApplication.Algorithms.Group
{
    public static class GroupSolver
    {
        public static IEnumerable<SinglePaymentRecord> FindGroupSettlement(SinglePurposeUserGroup group)
        {
            var userBalances = group.GetGroupRecords().GetUserBalances();

            var result = new List<SinglePaymentRecord>();
            while (userBalances.Any(pair => pair.Value != decimal.Zero))
            {
                /*
                 * Do while you have vertices with nonzero balance
                 * 1. Find a user who paid most and user who receiver most
                 * 2. Do a payment from the user, who received the most to the user, which paid most - value the user is missing
                 * 3. You have successfully removed one user from the graph
                 */
                var minValue = userBalances.Min(pair => pair.Value);
                if (minValue > 0)
                    throw new InvalidOperationException($"{nameof(minValue)} cannot be positive");

                var maxValue = userBalances.Max(pair => pair.Value);
                if (maxValue < 0)
                    throw new InvalidOperationException($"{nameof(maxValue)} cannot be negative");

                // Biggest dept from fellow users
                var (minValueUser, _) = userBalances.FirstOrDefault(pair => pair.Value == minValue);
                // Highest borrow
                var (maxValueUser, _) = userBalances.FirstOrDefault(pair => pair.Value == maxValue);

                //This payment settles at least one user, thus the loop will end
                var payment = new SinglePaymentRecord
                {
                    PaymentBy = maxValueUser,
                    PaymentFor = minValueUser,
                    Price = Math.Min(-minValue, maxValue)
                };

                userBalances[minValueUser] += payment.Price;
                userBalances[maxValueUser] -= payment.Price;
                result.Add(payment);
            }

            return result;
        }

        private static IEnumerable<SinglePaymentRecord> GetGroupRecords(this SinglePurposeUserGroup userGroup)
        {
            return userGroup.GroupPayments
                .SelectMany(paymentGroup => paymentGroup.PaymentTargets
                    .Select(singlePayment => new SinglePaymentRecord
                    {
                        PaymentBy = paymentGroup.PaymentBy,
                        PaymentFor = singlePayment.Target,
                        Price = singlePayment.Price
                    })
                );
        }
        
        private static Dictionary<ApplicationUser, decimal> GetUserBalances(
            this IEnumerable<SinglePaymentRecord> payments)
        {
            var userBalances = new Dictionary<ApplicationUser, decimal>();
            foreach (var payment in payments)
            {
                if (!userBalances.ContainsKey(payment.PaymentBy))
                    userBalances.TryAdd(payment.PaymentBy, 0);

                if (!userBalances.ContainsKey(payment.PaymentFor))
                    userBalances.TryAdd(payment.PaymentFor, 0);

                userBalances[payment.PaymentBy] -= payment.Price;
                userBalances[payment.PaymentFor] += payment.Price;
            }

            return userBalances;
        }
    }
}