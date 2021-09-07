using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Dto.GroupDto;
using Entities.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Algorithms.Group;
using WebApplication.Checks;
using WebApplication.Helpers;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationContext _context;

        private static readonly PropertyEqualityComparer<ApplicationUser, string> UserIdComparer =
            new(user => user.Id);

        private static readonly PropertyEqualityComparer<UnidirectionalPaymentGroup, Guid> PaymentGroupIdComparer =
            new(group => group.Id);

        private static readonly PropertyEqualityComparer<SinglePayment, Guid> SinglePaymentIdComparer =
            new(payment => payment.Id);

        public GroupController(UserManager<ApplicationUser> userManager, ApplicationContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        /// Returns the groups (and the group data), which is current user member of 
        /// </summary>
        [HttpGet("currentUserGroup")]
        public async Task<ActionResult<IEnumerable<SinglePurposeUserGroupDto>>> GetCurrentUserGroups()
        {
            var currentUserId = _userManager.GetUserId(User);
            var user = await _context.Users
                .AsSplitQuery()
                .Where(user => user.Id == currentUserId)
                .Include(user => user.PaymentGroups)
                .ThenInclude(paymentGroup => paymentGroup.GroupUsers)
                .Include(user => user.PaymentGroups)
                .ThenInclude(paymentGroup => paymentGroup.GroupPayments)
                .ThenInclude(unidirectionalPaymentGroup => unidirectionalPaymentGroup.PaymentTargets)
                .FirstOrDefaultAsync();

            Check.NotNull(user, "User not found");

            var data = user.PaymentGroups
                ?.Select(paymentGroup => paymentGroup.Serialize())
                ?.ToArray();
            return data;
        }

        /// <summary>
        /// Creates a new group and sets the user as the only user of the group
        /// </summary>
        /// <param name="groupDto"></param>
        /// <returns></returns>
        [HttpPost("addNewGroup")]
        public async Task<ActionResult<SinglePurposeUserGroupDto>> AddNewDefaultGroup(
            [FromBody] SinglePurposeUserGroupDto groupDto)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var group = new SinglePurposeUserGroup
            {
                Name = groupDto.Name,
                GroupUsers = new List<ApplicationUser> { currentUser }
            };

            await _context.UserGroups.AddAsync(group);

            await _context.SaveChangesAsync();
            return group.Serialize();
        }

        /// <summary>
        /// Modifies the group. If some value is null, no change is made.
        /// Changes to scalar types are done precisely, changes on the reference types are done on the level of added and
        /// removed entities (added entity is created and vice versa). Also everywhere where ApplicationUser is requested
        /// the user must exists.
        /// </summary>
        /// <param name="modifiedGroup"></param>
        /// <returns>Modified group</returns>
        [HttpPatch("modifyUserGroup")]
        public async Task<ActionResult<SinglePurposeUserGroupDto>> ModifyUserGroup(
            [FromBody] SinglePurposeUserGroupDto modifiedGroup)
        {
            Check.Guard(modifiedGroup.Id != Guid.Empty, "No group id provided");

            var foundGroup = await _context.UserGroups
                .Where(group => group.Id == modifiedGroup.Id)
                .Include(group => group.GroupUsers)
                .Include(group => group.GroupPayments)
                .FirstOrDefaultAsync();

            var currentUserId = _userManager.GetUserId(User);

            Check.NotNull(foundGroup, "Group does not exists");
            Check.Guard(foundGroup.GroupUsers.Any(user => user.Id == currentUserId),
                "You have to be part of the group to modify it");

            if (modifiedGroup.Name != null)
            {
                foundGroup.Name = modifiedGroup.Name;
            }

            var groupUserUpdate = foundGroup.GroupUsers.CalculateUpdate(modifiedGroup.GroupUsers,
                user => user.Deserialize(deserializeRequests: false), UserIdComparer);
            var paymentGroupUpdate = foundGroup.GroupPayments.CalculateUpdate(modifiedGroup.GroupPayments,
                SerializerExtensions.Deserialize, PaymentGroupIdComparer);

            var addedGroupUserIds = paymentGroupUpdate.Added
                .SelectMany(group =>
                    group.PaymentTargets.Select(target => target.Target.Id).Append(group.PaymentBy.Id));
            var usedUserIds = groupUserUpdate.Added
                .Select(user => user.Id)
                .Concat(addedGroupUserIds)
                .ToHashSet();

            var loadedUsers = await _context.Users
                .Where(user => usedUserIds.Contains(user.Id))
                .ToDictionaryAsync(user => user.Id);

            foundGroup.GroupUsers.RemoveAll(groupUserUpdate.Removed);
            foundGroup.GroupUsers.AddAll(groupUserUpdate.Added, user => loadedUsers[user.Id]);

            foundGroup.GroupPayments.RemoveAll(paymentGroupUpdate.Removed);
            foundGroup.GroupPayments.AddAll(paymentGroupUpdate.Added, group =>
            {
                group.PaymentBy = loadedUsers[group.PaymentBy.Id];
                foreach (var targetPayment in group.PaymentTargets)
                {
                    targetPayment.Target = loadedUsers[targetPayment.Target.Id];
                }

                return group;
            });

            await _context.SaveChangesAsync();
            return foundGroup.Serialize();
        }

        /// <summary>
        /// Modify the payment group, by updating all not null provided values. If updated property is scalar, property is updated as is
        /// If the property is collection, only changes in the terms of added/removed entity is performed
        /// </summary>
        /// <param name="modifiedPaymentGroup">body request</param>
        /// <returns>Updated payment group</returns>
        [HttpPatch("modifyPaymentGroup")]
        public async Task<ActionResult<UnidirectionalPaymentGroupDto>> ModifyPaymentGroup(
            [FromBody] UnidirectionalPaymentGroupDto modifiedPaymentGroup)
        {
            Check.Guard(modifiedPaymentGroup.Id != Guid.Empty, "Payment group id not provided");

            var foundPaymentGroup = await _context.PaymentGroups
                .AsSplitQuery()
                .Where(paymentGroup => paymentGroup.Id == modifiedPaymentGroup.Id)
                .Include(paymentGroup => paymentGroup.PaymentBy)
                .Include(paymentGroup => paymentGroup.PaymentTargets)
                .ThenInclude(singlePayment => singlePayment.Target)
                .Include(paymentGroup => paymentGroup.UserGroup)
                .ThenInclude(userGroup => userGroup.GroupUsers)
                .FirstOrDefaultAsync();

            Check.NotNull(foundPaymentGroup, "Payment group not found");

            var currentUserId = _userManager.GetUserId(User);

            Check.Guard(foundPaymentGroup.UserGroup.GroupUsers.Any(user => user.Id == currentUserId),
                "You are not a member of a group");

            var paymentUpdate = foundPaymentGroup.PaymentTargets.CalculateUpdate(modifiedPaymentGroup.PaymentTargets,
                SerializerExtensions.Deserialize, SinglePaymentIdComparer);

            var addedUserIds = paymentUpdate.Added
                .Select(payment => payment.Target.Id)
                .Append(modifiedPaymentGroup.PaymentBy?.Id)
                .Where(userId => userId != null)
                .ToHashSet();
            var loadedUsers = await _context.Users
                .Where(user => addedUserIds.Contains(user.Id))
                .ToDictionaryAsync(user => user.Id);

            if (modifiedPaymentGroup.Name != null)
            {
                foundPaymentGroup.Name = modifiedPaymentGroup.Name;
            }

            if (modifiedPaymentGroup.PaymentBy != null)
            {
                foundPaymentGroup.PaymentBy = loadedUsers[modifiedPaymentGroup.PaymentBy.Id];
            }

            foundPaymentGroup.PaymentTargets.RemoveAll(paymentUpdate.Removed);
            foundPaymentGroup.PaymentTargets.AddAll(paymentUpdate.Added, payment =>
            {
                payment.Target = loadedUsers[payment.Target.Id];
                return payment;
            });

            await _context.SaveChangesAsync();
            return foundPaymentGroup.Serialize();
        }

        /// <summary>
        /// Modifies single payment. Changes the data based on properties given in body. If the value is null,
        /// no change is made
        /// </summary>
        /// <param name="paymentDto">body request</param>
        /// <returns>Updated single payment</returns>
        [HttpPatch("modifySinglePayment")]
        public async Task<ActionResult<SinglePaymentDto>> ModifySinglePayment([FromBody] SinglePaymentDto paymentDto)
        {
            Check.Guard(paymentDto.Id != Guid.Empty, "Single payment id not provided");

            var foundPayment = await _context.SinglePayments
                .Where(payment => payment.Id == paymentDto.Id)
                .Include(payment => payment.Target)
                .Include(payment => payment.PaymentGroup)
                .ThenInclude(paymentGroup => paymentGroup.UserGroup)
                .ThenInclude(userGroup => userGroup.GroupUsers)
                .FirstOrDefaultAsync();
            Check.NotNull(foundPayment, "Single payment not found");

            var currentUserId = _userManager.GetUserId(User);
            Check.Guard(foundPayment.PaymentGroup.UserGroup.GroupUsers.Any(user => user.Id == currentUserId),
                "You have to be part of the group to modify it");

            foundPayment.Price = paymentDto.Price;
            if (paymentDto.Target?.Id != null)
            {
                var userReplacement = await _context.Users.FindAsync(paymentDto.Target.Id);
                foundPayment.Target = userReplacement;
            }

            await _context.SaveChangesAsync();
            return paymentDto;
        }
        
        /// <summary>
        /// Calculates group settlement, the list of transactions, which if performed, the group will be even
        /// </summary>
        /// <param name="groupId">group id</param>
        /// <returns>Group settlement as list of transactions</returns>
        [HttpGet("getGroupSettlement/{groupId:guid}")]
        public async Task<ActionResult<IEnumerable<PaymentRecordDto>>> GetGroupBalance(Guid groupId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var group = await _context.UserGroups
                .Where(group => group.Id == groupId)
                .AsSplitQuery()
                .Include(group => group.GroupUsers)
                .Include(group => group.GroupPayments)
                .ThenInclude(groupPayment => groupPayment.PaymentBy)
                .Include(group => group.GroupPayments)
                .ThenInclude(group => group.PaymentTargets)
                .ThenInclude(target => target.Target)
                .FirstOrDefaultAsync();
            Check.Guard(group.GroupUsers.Any(user => user.Id == currentUserId),
                "You have to be part of the group to get the group settlement");

            var groupSettlement = GroupSolver.FindGroupSettlement(group);
            return groupSettlement
                .Select(record => new PaymentRecordDto
                {
                    PaymentBy = record.PaymentBy.Serialize(serializeRequests: false),
                    PaymentFor = record.PaymentFor.Serialize(serializeRequests: false),
                    Price = record.Price
                })
                .ToArray();
        }
    }
}