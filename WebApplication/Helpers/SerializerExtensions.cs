using System.Linq;
using Entities.Dto.AuthDto;
using Entities.Dto.GroupDto;
using WebApplication.Models;

namespace WebApplication.Helpers
{
    /// <summary>
    /// Basic serialization and deserialization methods for all DTOs used.
    /// </summary>
    public static class SerializerExtensions
    {
        /// <summary>
        /// Serialization of ApplicationUser model into Dto
        /// </summary>
        /// <param name="user">ApplicationUser to deserialize</param>
        /// <param name="serializeRequests">Parameter used to prevent circular serialization, friendship requests are not serializing users request</param>
        /// <returns></returns>
        public static UserDto Serialize(this ApplicationUser user, bool serializeRequests = false)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                IncomingRequests =
                    serializeRequests ? user.IncomingRequests?.Select(status => status.Serialize()) : null,
                SentRequests = serializeRequests ? user.SentRequests?.Select(status => status.Serialize()) : null,
            };
        }

        /// <summary>
        /// Deserialization of UserDto into ApplicationUser
        /// </summary>
        /// <param name="user">Dto to deserialize</param>
        /// <param name="deserializeRequests">Parameter used to prevent circular deserialization, friendship requests are not serializing users request</param>
        /// <returns>ApplicationUser instance</returns>
        public static ApplicationUser Deserialize(this UserDto user, bool deserializeRequests = true)
        {
            return new ApplicationUser
            {
                Id = user.Id,
                UserName = user.UserName,
                IncomingRequests = deserializeRequests
                    ? user.IncomingRequests?.Select(request => request.Deserialize()).ToList()
                    : null,
                SentRequests = deserializeRequests
                    ? user.SentRequests?.Select(request => request.Deserialize()).ToList()
                    : null,
            };
        }

        public static FriendRequestDto Serialize(this FriendshipStatus friendshipStatus)
        {
            return new FriendRequestDto
            {
                Id = friendshipStatus.Id,
                Created = friendshipStatus.Created,
                Modified = friendshipStatus.Modified,
                State = friendshipStatus.State,
                From = friendshipStatus.From?.Serialize(),
                To = friendshipStatus.To?.Serialize()
            };
        }

        public static FriendshipStatus Deserialize(this FriendRequestDto friendshipStatus)
        {
            return new FriendshipStatus
            {
                Id = friendshipStatus.Id,
                Created = friendshipStatus.Created,
                Modified = friendshipStatus.Modified,
                State = friendshipStatus.State,
                From = friendshipStatus.From?.Deserialize(),
                To = friendshipStatus.To?.Deserialize()
            };
        }

        public static SinglePurposeUserGroupDto Serialize(this SinglePurposeUserGroup group)
        {
            return new SinglePurposeUserGroupDto
            {
                Id = group.Id,
                Name = group.Name,
                GroupUsers = group.GroupUsers?.Select(user => user.Serialize()),
                GroupPayments = group.GroupPayments?.Select(paymentGroup => paymentGroup.Serialize())
            };
        }

        public static SinglePurposeUserGroup Deserialize(this SinglePurposeUserGroupDto group)
        {
            return new SinglePurposeUserGroup
            {
                Id = group.Id,
                Name = group.Name,
                GroupUsers = group.GroupUsers?.Select(user => user.Deserialize()).ToList(),
                GroupPayments = group.GroupPayments?.Select(paymentGroup => paymentGroup.Deserialize()).ToList()
            };
        }

        public static UnidirectionalPaymentGroupDto Serialize(this UnidirectionalPaymentGroup paymentGroup)
        {
            return new UnidirectionalPaymentGroupDto
            {
                Id = paymentGroup.Id,
                Name = paymentGroup.Name,
                PaymentBy = paymentGroup.PaymentBy.Serialize(),
                PaymentTargets = paymentGroup.PaymentTargets?.Select(paymentTarget => paymentTarget.Serialize())
            };
        }

        public static UnidirectionalPaymentGroup Deserialize(this UnidirectionalPaymentGroupDto paymentGroup)
        {
            return new UnidirectionalPaymentGroup
            {
                Id = paymentGroup.Id,
                Name = paymentGroup.Name,
                PaymentBy = paymentGroup.PaymentBy.Deserialize(),
                PaymentTargets = paymentGroup.PaymentTargets?.Select(paymentTarget => paymentTarget.Deserialize())
                    .ToList()
            };
        }

        public static SinglePaymentDto Serialize(this SinglePayment payment)
        {
            return new SinglePaymentDto
            {
                Id = payment.Id,
                Price = payment.Price,
                Target = payment.Target.Serialize()
            };
        }

        public static SinglePayment Deserialize(this SinglePaymentDto paymentDto)
        {
            return new SinglePayment
            {
                Id = paymentDto.Id,
                Price = paymentDto.Price,
                Target = paymentDto.Target.Deserialize()
            };
        }
    }
}