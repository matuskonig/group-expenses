using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication.Models;

namespace WebApplication.Algorithms.Group
{
    public static class GroupSolver
    {
        public static IEnumerable<SinglePaymentRecord> FindGroupSettlement(SinglePurposeUserGroup group)
        {
            var userBalances = group.GetGroupRecords().GetUserBalances();

            var result = new List<SinglePaymentRecord>();

            // Algorithm:
            //      1. Find the user, who received and paid most money
            //      2. Create a payment from user who received most to user who paid most in value setting balance of at
            //          least one user to zero
            //      3. If not all users have zero balance, go to step 1

            // Loop ends when all people have balance equal of zero - have no depth or paid money
            while (userBalances.Any(pair => pair.Value != decimal.Zero))
            {
                /*
                 * Do while you have vertices with nonzero balance
                 * 1. Find a user who paid most and user who receiver most
                 * 2. Do a payment from the user, who received the most to the user, which paid most - value the user is missing
                 * 3. You have successfully removed one user from the graph
                 */

                // The biggest paid money value in current balances
                var minValue = userBalances.Min(pair => pair.Value);
                if (minValue > 0)
                    throw new InvalidOperationException($"{nameof(minValue)} cannot be positive");

                // The biggest received money value in current balances
                var maxValue = userBalances.Max(pair => pair.Value);
                if (maxValue < 0)
                    throw new InvalidOperationException($"{nameof(maxValue)} cannot be negative");

                // User who paid the most for the users
                var (minValueUser, _) = userBalances.FirstOrDefault(pair => pair.Value == minValue);
                // User who received the most money from the other users
                var (maxValueUser, _) = userBalances.FirstOrDefault(pair => pair.Value == maxValue);

                // We create a payment from the user who owes the most money to the user who borrowed the most money
                // (user with maximum positive and minimum negative balance)
                // The transaction holds the minimal owed value, resulting in setting at least one user balance to zero
                // Usage of maximal owed can be later considered, it can result in better UX in inconsistent graph,
                // where only a small amount of users are money receivers
                var payment = new SinglePaymentRecord
                {
                    PaymentBy = maxValueUser,
                    PaymentFor = minValueUser,
                    Price = Math.Min(-minValue, maxValue)
                };

                userBalances[maxValueUser] -= payment.Price;
                userBalances[minValueUser] += payment.Price;

                result.Add(payment);
            }

            return result;
        }

        /// <summary>
        /// Creates a list of transactions between the users
        /// </summary>
        /// <param name="userGroup"></param>
        /// <returns>List of transactions</returns>
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

        /// <summary>
        /// Given the list of all transactions, calculate the mapping of user and its total balance.
        /// When payment is made, user sending money gets negative balance, whereas user receiving money receives positive
        /// balance. This is summed out for every transaction and gives overall balance
        /// </summary>
        /// <param name="payments">list of payments</param>
        /// <returns>User - overall balance mapping</returns>
        private static Dictionary<ApplicationUser, decimal> GetUserBalances(
            this IEnumerable<SinglePaymentRecord> payments)
        {
            var userBalances = new Dictionary<ApplicationUser, decimal>();
            foreach (var payment in payments)
            {
                // Default map initialization
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