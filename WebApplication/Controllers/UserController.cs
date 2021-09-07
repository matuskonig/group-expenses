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

        /// <summary>
        /// Creates a new friend request (FriendshipStatus) between currentUser and user by id
        /// </summary>
        /// <param name="id">user id of the other user, the request target</param>
        /// <returns>newly created FriendshipStatus</returns>
        [HttpGet("sendNewRequest/{id}")]
        public async Task<ActionResult<FriendshipStatusDto>> SendNewFriendRequest(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            var otherUser = await _context.Users.FindAsync(id);
            var possibleExistingFriendshipStatus = await _context.FriendRequests
                .FirstOrDefaultAsync(friendshipStatus =>
                    friendshipStatus.From == user && friendshipStatus.To == otherUser &&
                    friendshipStatus.State != FriendRequestState.Rejected);
            Check.NotNull(otherUser, $"Other user does not exist");
            Check.Null(possibleExistingFriendshipStatus, "Request to this user has already been sent");
            Check.Guard(user.Id != otherUser.Id, "You cant send request to yourself");

            var status = new FriendshipStatus
            {
                From = user,
                To = otherUser,
                State = FriendRequestState.WaitingForAccept,
                Created = DateTime.Now,
                Modified = DateTime.Now
            };
            _context.FriendRequests.Add(status);

            await _context.SaveChangesAsync();
            return status.Serialize();
        }

        /// <summary>
        /// Accepts the friend request by setting FriendshipStatus state
        /// </summary>
        /// <param name="id">FriendshipStatus id</param>
        /// <returns>Updated FriendshipStatus</returns>
        [HttpGet("acceptRequest/{id:guid}")]
        public async Task<ActionResult<FriendshipStatusDto>> AcceptFriendRequest(Guid id)
        {
            var friendStatus = await _context.FriendRequests
                .Where(status => status.Id == id)
                .Include(status => status.From)
                .Include(status => status.To)
                .FirstOrDefaultAsync();
            var currentUserId = _userManager.GetUserId(User);
            Check.NotNull(friendStatus, "Friend request does not exist");
            Check.Guard(friendStatus.To.Id == currentUserId, "Friend request was sent to other than current user");

            friendStatus.State = FriendRequestState.Accepted;
            friendStatus.Modified = DateTime.Now;

            await _context.SaveChangesAsync();
            return friendStatus.Serialize();
        }

        /// <summary>
        /// Rejects the friend request by setting state of FriendshipStatus
        /// </summary>
        /// <param name="id">Request id</param>
        /// <returns>FriendshipStatus after changes are made</returns>
        [HttpGet("rejectRequest/{id:guid}")]
        public async Task<ActionResult<FriendshipStatusDto>> RejectFriendRequest(Guid id)
        {
            var request = await _context.FriendRequests.FindAsync(id);
            Check.NotNull(request, "Friend request not found");
            Check.Guard(request.State != FriendRequestState.Rejected, "Friend request has already been rejected");

            request.State = FriendRequestState.Rejected;

            await _context.SaveChangesAsync();
            return request.Serialize();
        }

        /// <summary>
        /// Performs a search query by parameters given
        /// </summary>
        /// <param name="request"></param>
        /// <returns>List of users satisfying individual searches</returns>
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
                UsersByUserName = usersByUserName?.Select(user => user.Serialize())
            };
        }

        /// <summary>
        /// Returns the current user with all data loaded
        /// </summary>
        /// <returns>Serialized current user with all data loaded</returns>
        [HttpGet("current")]
        public async Task<ActionResult<UserDto>> GetCurrent()
        {
            var currentUserId = _userManager.GetUserId(User);
            var user = await _context.Users
                .AsSplitQuery()
                .Where(user => user.Id == currentUserId)
                .Include(applicationUser => applicationUser.IncomingRequests)
                .ThenInclude(friendshipStatus => friendshipStatus.From)
                .Include(applicationUser => applicationUser.SentRequests)
                .ThenInclude(friendshipStatus => friendshipStatus.To)
                .FirstOrDefaultAsync();

            Check.NotNull(user, "User does not exist");
            return user.Serialize(serializeRequests: true);
        }
    }
}