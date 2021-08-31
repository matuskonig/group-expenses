using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication.Authentication;
using WebApplication.Models;

namespace WebApplication.Algorithms
{
    public static class GroupSolver
    {
        // TODO: refactor
        public static List<(ApplicationUser PaymentBy, ApplicationUser PaymentFor, decimal Price)> FindGroupSettlement(
            SinglePurposeUserGroup group)
        {
            var paymentGraphEdges = group
                .GetGroupRecords()
                .FixWrongInput()
                .MergePaymentsInOneDirection()
                .MergePaymentsInOpposingDirection()
                .ToHashSet();

            var outEdges = paymentGraphEdges
                .GroupBy(payment => payment.PaymentBy)
                .ToDictionary(grouping => grouping.Key, grouping => grouping);
            var inEdges = paymentGraphEdges
                .GroupBy(payment => payment.PaymentFor)
                .ToDictionary(grouping => grouping.Key, grouping => grouping);
            var userPaidBalance = outEdges.ToDictionary(pair => pair.Key,
                pair => -pair.Value.Sum(paymentRecord => paymentRecord.Price));
            var userReceived = inEdges.ToDictionary(pair => pair.Key,
                pair => pair.Value.Sum(paymentRecord => paymentRecord.Price));
            var totalBalance = userPaidBalance
                .Union(userReceived)
                .GroupBy(pair => pair.Key)
                .ToDictionary(grouping => grouping.Key, grouping => grouping.Sum(pair => pair.Value));


            var result = new List<(ApplicationUser PaymentBy, ApplicationUser PaymentFor, decimal Price)>();
            while (totalBalance.Any(pair => pair.Value != decimal.Zero))
            {
                /*
                 * Do while you have verticies with nonzero balance
                 * 1. Find a user who paid most and user who receiver most
                 * 2. Do a payment from the user, who received the most to the user, which paid most - value the user is missing
                 * 3. You have successfully removed one user from the graph
                 */
                var minValue = totalBalance.Min(pair => pair.Value);
                if (minValue > 0)
                    throw new InvalidOperationException($"{nameof(minValue)} cannot be positive");
                var maxValue = totalBalance.Max(pair => pair.Value);
                if (maxValue < 0)
                    throw new InvalidOperationException($"{nameof(maxValue)} cannot be negative");

                // Biggest dept from fellow users
                var (minValueUser, _) = totalBalance.FirstOrDefault(pair => pair.Value == minValue);
                // Highest borrow
                var (maxValueUser, _) = totalBalance.FirstOrDefault(pair => pair.Value == maxValue);

                //This payment settles at least one user, thus the loop will end
                var payment = (maxValueUser, minValueUser, Price: Math.Min(-minValue, maxValue));

                totalBalance[minValueUser] += payment.Price;
                totalBalance[maxValueUser] -= payment.Price;
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

        private static IEnumerable<SinglePaymentRecord> MergePaymentsInOneDirection(
            this IEnumerable<SinglePaymentRecord> records)
        {
            return records
                .GroupBy(singlePaymentRecord =>
                    (From: singlePaymentRecord.PaymentBy, To: singlePaymentRecord.PaymentFor))
                .Select(grouping =>
                    new SinglePaymentRecord
                    {
                        PaymentBy = grouping.Key.From,
                        PaymentFor = grouping.Key.To,
                        Price = grouping.Sum(paymentRecord => paymentRecord.Price)
                    });
        }

        private static IEnumerable<SinglePaymentRecord> FixWrongInput(
            this IEnumerable<SinglePaymentRecord> records)
        {
            return records
                .Select(record => record.Price switch
                {
                    <0 => record with
                    {
                        PaymentFor = record.PaymentBy,
                        PaymentBy = record.PaymentFor,
                        Price = -record.Price
                    },
                    0 => null,
                    _ => record
                })
                .Where(record => record != null);
        }

        private static IEnumerable<SinglePaymentRecord> MergePaymentsInOpposingDirection(
            this IEnumerable<SinglePaymentRecord> records)
        {
            var paymentRecordsSource = records.ToDictionary(record => (record.PaymentBy, record.PaymentFor));
            return paymentRecordsSource.Select(pair =>
                {
                    var ((paymentBy, paymentFor), paymentRecord) = pair;
                    // If there exist no opposite edge -> return the single route
                    if (!paymentRecordsSource.TryGetValue((paymentFor, paymentBy), out var otherPaymentRecord))
                        return paymentRecord;

                    var difference = paymentRecord.Price - otherPaymentRecord.Price;
                    return difference switch
                    {
                        >0 => paymentRecord with { Price = difference },
                        <0 => otherPaymentRecord with { Price = -difference },
                        0 => null,
                    };
                })
                .Where(paymentRecord => paymentRecord != null);
        }


        public record SinglePaymentRecord
        {
            public ApplicationUser PaymentBy { get; init; }
            public ApplicationUser PaymentFor { get; init; }
            public decimal Price { get; init; }
        }
    }
}