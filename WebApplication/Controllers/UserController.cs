using System;
using System.Linq;
using System.Threading.Tasks;
using Entities.Dto.AuthDto;
using Entities.Dto.UserDto;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Authentication;
using WebApplication.Checks;
using WebApplication.Helpers;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationContext _context;

        public UserController(UserManager<ApplicationUser> userManager, ApplicationContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("sendNewRequest/{id}")]
        public async Task<ActionResult<FriendRequestDto>> SendNewFriendRequest(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            var otherUser = await _context.Users.FindAsync(id);
            var possibleExistingRelation = await _context.FriendRequests
                .FirstOrDefaultAsync(request =>
                    request.From == user && request.To == otherUser &&
                    request.State != FriendRequestState.Rejected);
            Check.NotNull(otherUser, $"Other user does not exist");
            Check.Null(possibleExistingRelation, "Request to this user has already been sent");
            Check.Guard(user.Id != otherUser.Id, "You cant send request to yourself");

            var relation = new FriendshipStatus
            {
                From = user,
                To = otherUser,
                State = FriendRequestState.WaitingForAccept,
                Created = DateTime.Now,
                Modified = DateTime.Now
            };
            _context.FriendRequests.Add(relation);
            await _context.SaveChangesAsync();
            return relation.Serialize();
        }

        [HttpGet("acceptRequest/{id:guid}")]
        public async Task<ActionResult<FriendRequestDto>> AcceptFriendRequest(Guid id)
        {
            var friendRequest = await _context.FriendRequests
                .Where(status => status.Id == id)
                .Include(status => status.From)
                .Include(status => status.To)
                .FirstOrDefaultAsync();
            var currentUser = await _userManager.GetUserAsync(User);
            Check.NotNull(friendRequest, "Friend request does not exist");
            Check.Guard(friendRequest.To == currentUser, "Friend request was sent to other than current user");

            friendRequest.State = FriendRequestState.Accepted;
            friendRequest.Modified = DateTime.Now;

            await _context.SaveChangesAsync();
            return friendRequest.Serialize();
        }

        [HttpGet("rejectRequest/{id:guid}")]
        public async Task<ActionResult<FriendRequestDto>> RejectFriendRequest(Guid id)
        {
            var request = await _context.FriendRequests.FindAsync(id);
            Check.NotNull(request, "Friend request not found");
            Check.Guard(request.State != FriendRequestState.Rejected, "Friend request has already been rejected");

            request.State = FriendRequestState.Rejected;
            await _context.SaveChangesAsync();
            return request.Serialize();
        }

        [HttpPost("searchUser")]
        public async Task<ActionResult<SearchUserResponse>> SearchFriendByName([FromBody] SearchUserRequest request)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var usersByUserName = request.UserName == null
                ? null
                : await _context.Users
                    .Where(applicationUser => applicationUser.UserName.Contains(request.UserName) &&
                                              applicationUser.UserName != currentUser.UserName)
                    .ToListAsync();

            return new SearchUserResponse
            {
                UsersByUserName = usersByUserName?.Select(user => user.Serialize(serializeRequests: false))
            };
        }

        [HttpGet("current")]
        public async Task<ActionResult<UserDto>> GetCurrent()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .AsSplitQuery()
                .Where(u => u.Id == userId)
                .Include(applicationUser => applicationUser.IncomingRequests)
                .ThenInclude(friendshipStatus => friendshipStatus.From)
                .Include(applicationUser => applicationUser.SentRequests)
                .ThenInclude(friendshipStatus => friendshipStatus.To)
                .FirstOrDefaultAsync();
            Check.NotNull(user, "User does not exist");
            return user.Serialize();
        }
    }
}