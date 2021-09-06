using System.Linq;
using Entities.Dto.AuthDto;
using Entities.Dto.GroupDto;
using WebApplication.Authentication;
using WebApplication.Models;

namespace WebApplication.Helpers
{
    public static class SerializerExtensions
    {
        public static UserDto Serialize(this ApplicationUser user, bool serializeRequests = true)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                IncomingRequests = serializeRequests ? user.IncomingRequests?.Select(Serialize) : null,
                SentRequests = serializeRequests ? user.SentRequests?.Select(Serialize) : null,
            };
        }

        public static ApplicationUser Deserialize(this UserDto user, bool deserializeRequests = true)
        {
            return new ApplicationUser
            {
                Id = user.Id,
                UserName = user.UserName,
                IncomingRequests = deserializeRequests ? user.IncomingRequests?.Select(Deserialize).ToList() : null,
                SentRequests = deserializeRequests ? user.SentRequests?.Select(Deserialize).ToList() : null,
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
                From = friendshipStatus.From?.Serialize(serializeRequests: false),
                To = friendshipStatus.To?.Serialize(serializeRequests: false)
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
                From = friendshipStatus.From?.Deserialize(deserializeRequests: false),
                To = friendshipStatus.To?.Deserialize(deserializeRequests: false)
            };
        }

        public static SinglePurposeUserGroupDto Serialize(this SinglePurposeUserGroup group)
        {
            return new SinglePurposeUserGroupDto
            {
                Id = group.Id,
                Name = group.Name,
                GroupUsers = group.GroupUsers?.Select(user => user.Serialize(serializeRequests: false)),
                GroupPayments = group.GroupPayments?.Select(Serialize)
            };
        }

        public static SinglePurposeUserGroup Deserialize(this SinglePurposeUserGroupDto group)
        {
            return new SinglePurposeUserGroup
            {
                Id = group.Id,
                Name = group.Name,
                GroupUsers = group.GroupUsers?.Select(user => user.Deserialize()).ToList(),
                GroupPayments = group.GroupPayments?.Select(Deserialize).ToList()
            };
        }

        public static UnidirectionalPaymentGroupDto Serialize(this UnidirectionalPaymentGroup paymentGroup)
        {
            return new UnidirectionalPaymentGroupDto
            {
                Id = paymentGroup.Id,
                Name = paymentGroup.Name,
                PaymentBy = paymentGroup.PaymentBy.Serialize(),
                PaymentTargets = paymentGroup.PaymentTargets?.Select(Serialize)
            };
        }

        public static UnidirectionalPaymentGroup Deserialize(this UnidirectionalPaymentGroupDto paymentGroup)
        {
            return new UnidirectionalPaymentGroup
            {
                Id = paymentGroup.Id,
                Name = paymentGroup.Name,
                PaymentBy = paymentGroup.PaymentBy.Deserialize(),
                PaymentTargets = paymentGroup.PaymentTargets?.Select(Deserialize).ToList()
            };
        }

        public static SinglePaymentDto Serialize(this SinglePayment payment)
        {
            return new SinglePaymentDto
            {
                Id = payment.Id,
                Price = payment.Price,
                Target = payment.Target.Serialize(serializeRequests: false)
            };
        }

        public static SinglePayment Deserialize(this SinglePaymentDto paymentDto)
        {
            return new SinglePayment
            {
                Id = paymentDto.Id,
                Price = paymentDto.Price,
                Target = paymentDto.Target.Deserialize(deserializeRequests: false)
            };
        }
    }
}