using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.GroupDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Algorithms.Group;
using WebApplication.Authentication;
using WebApplication.Checks;
using WebApplication.Helpers;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationContext _context;

        public GroupController(UserManager<ApplicationUser> userManager, ApplicationContext context)
        {
            _userManager = userManager;
            _context = context;
        }

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

            var (addedUsers, deletedUsers) = modifiedGroup.GroupUsers == null
                ? (Enumerable.Empty<ApplicationUser>(), Enumerable.Empty<ApplicationUser>())
                : foundGroup
                    .GroupUsers
                    .CalculateUpdate(modifiedGroup.GroupUsers, user => user.Id,
                        user => user.Id, user => user.Deserialize(deserializeRequests: false));
            var (addedGroups, deletedGroups) =
                modifiedGroup.GroupPayments == null
                    ? (Enumerable.Empty<UnidirectionalPaymentGroup>(), Enumerable.Empty<UnidirectionalPaymentGroup>())
                    : foundGroup
                        .GroupPayments
                        .CalculateUpdate(modifiedGroup.GroupPayments, group => group.Id, group => group.Id,
                            SerializerExtensions.Deserialize);

            var addedGroupUsers = addedGroups
                .SelectMany(group => group.PaymentTargets.Select(target => target.Target).Append(group.PaymentBy));
            var fak = addedUsers.Concat(addedGroupUsers).Select(user => user.Id).ToHashSet(); //TODO: rename
            var loadedUsers = await _context.Users
                .Where(user => fak.Contains(user.Id))
                .ToDictionaryAsync(user => user.Id);
            foreach (var toRemove in deletedUsers)
            {
                foundGroup.GroupUsers.Remove(toRemove);
            }

            foreach (var toAdd in addedUsers)
            {
                foundGroup.GroupUsers.Add(loadedUsers[toAdd.Id]);
            }


            foreach (var toRemove in deletedGroups)
            {
                foundGroup.GroupPayments.Remove(toRemove);
            }

            foreach (var toAdd in addedGroups)
            {
                toAdd.PaymentBy = loadedUsers[toAdd.PaymentBy.Id];
                foreach (var paymentTarget in toAdd.PaymentTargets)
                {
                    paymentTarget.Target = loadedUsers[paymentTarget.Target.Id];
                }

                foundGroup.GroupPayments.Add(toAdd);
            }

            await _context.SaveChangesAsync();
            return foundGroup.Serialize();
        }

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


            _context.Entry(foundPaymentGroup).OriginalValues
                .SetValues(modifiedPaymentGroup); //TODO: zistit, co to robi vlastne
            var (addedPayments, removedPayments) =
                modifiedPaymentGroup.PaymentTargets == null
                    ? (Enumerable.Empty<SinglePayment>(), Enumerable.Empty<SinglePayment>())
                    : foundPaymentGroup
                        .PaymentTargets
                        .CalculateUpdate(modifiedPaymentGroup.PaymentTargets, payment => payment.Id,
                            payment => payment.Id,
                            SerializerExtensions.Deserialize);
            var addedUserIds = addedPayments
                .Select(payment => payment.Target.Id)
                .Append(modifiedPaymentGroup.PaymentBy?.Id)
                .Where(value => value != null)
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

            if (modifiedPaymentGroup.PaymentTargets != null)
            {
                foreach (var deletedPayment in removedPayments)
                {
                    foundPaymentGroup.PaymentTargets.Remove(deletedPayment);
                }

                foreach (var addedPayment in addedPayments)
                {
                    addedPayment.Target = loadedUsers[addedPayment.Target.Id];
                    foundPaymentGroup.PaymentTargets.Add(addedPayment);
                }
            }

            await _context.SaveChangesAsync();
            return foundPaymentGroup.Serialize();
        }

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

        [HttpGet("getGroupSettlement/{groupId:guid}")]
        public async Task<ActionResult<IEnumerable<PaymentRecordDto>>> GetGroupBalance(Guid groupId)
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(user => user.Id == _userManager.GetUserId(User));
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
            Check.Guard(group.GroupUsers.Contains(currentUser),
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