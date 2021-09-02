using System;
using System.Linq;
using System.Threading.Tasks;
using Entities.AuthDto;
using Entities.Enums;
using Entities.UserDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Authentication;
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
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var otherUser = await _context.Users.FindAsync(id);
                var possibleExistingRelation = await _context.FriendRequests
                    .FirstOrDefaultAsync(request =>
                        request.From == user && request.To == otherUser && request.State != FriendRequestState.Rejected);
                if (otherUser == null || possibleExistingRelation != null || user.Id == otherUser.Id)
                    return BadRequest();

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
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("acceptRequest/{id:guid}")]
        public async Task<ActionResult<FriendRequestDto>> AcceptFriendRequest(Guid id)
        {
            try
            {
                var friendRequest = await _context.FriendRequests
                    .Where(status => status.Id == id)
                    .Include(status => status.From)
                    .Include(status => status.To)
                    .FirstOrDefaultAsync();
                var currentUser = await _userManager.GetUserAsync(User);

                if (friendRequest == null || friendRequest.To != currentUser)
                {
                    return BadRequest();
                }

                friendRequest.State = FriendRequestState.Accepted;
                friendRequest.Modified = DateTime.Now;

                await _context.SaveChangesAsync();
                return friendRequest.Serialize();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("rejectRequest/{id:guid}")]
        public async Task<ActionResult<FriendRequestDto>> RejectFriendRequest(Guid id)
        {
            try
            {
                var request = await _context.FriendRequests.FindAsync(id);
                if(request == null || request.State == FriendRequestState.Rejected)
                    return BadRequest();
                request.State = FriendRequestState.Rejected;
                await _context.SaveChangesAsync();
                return request.Serialize();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost("searchUser")]
        public async Task<ActionResult<SearchUserResponse>> SearchFriendByName([FromBody] SearchUserRequest request)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var usersByUserName = request.UserName == null
                ? null
                : await _context.Users
                    .Where(applicationUser => applicationUser.UserName.Contains(request.UserName) &&
                                              applicationUser.UserName != currentUser.UserName
                    )
                    .ToListAsync();
            return new SearchUserResponse
            {
                UsersByUserName = usersByUserName?.Select(user => user.Serialize())
            };
        }

        [Authorize]
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
            return user == null
                ? BadRequest()
                : user.Serialize();
        }
    }
}