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
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationContext context;

        public UserController(UserManager<ApplicationUser> userManager, ApplicationContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        [HttpGet("sendNewRequest/{id}")]
        public async Task<ActionResult> SendNewFriendRequest(string id)
        {
            try
            {
                var user = await userManager.GetUserAsync(User);
                var otherUser = await context.Users.FindAsync(id);
                var possibleExistingRelation = await context.FriendRequests
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
                context.FriendRequests.Add(relation);
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [Authorize]
        [HttpGet("acceptRequest/{id:guid}")]
        public async Task<ActionResult<FriendRequestDto>> AcceptFriendRequest(Guid id)
        {
            try
            {
                var friendRequest = await context.FriendRequests
                    .Where(status => status.Id == id)
                    .Include(status => status.From)
                    .Include(status => status.To)
                    .FirstOrDefaultAsync();
                var currentUser = await userManager.GetUserAsync(User);

                if (friendRequest == null || friendRequest.To != currentUser)
                {
                    return BadRequest();
                }

                friendRequest.State = FriendRequestState.Accepted;
                friendRequest.Modified = DateTime.Now;

                await context.SaveChangesAsync();
                return friendRequest.Serialize();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [Authorize]
        [HttpGet("rejectRequest/{id:guid}")]
        public async Task<ActionResult<FriendRequestDto>> RejectFriendRequest(Guid id)
        {
            try
            {
                var request = await context.FriendRequests.FindAsync(id);
                if(request == null || request.State == FriendRequestState.Rejected)
                    return BadRequest();
                request.State = FriendRequestState.Rejected;
                await context.SaveChangesAsync();
                return request.Serialize();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [Authorize]
        [HttpPost("searchUser")]
        public async Task<ActionResult<SearchUserResponse>> SearchFriendByName([FromBody] SearchUserRequest request)
        {
            var currentUser = await userManager.GetUserAsync(User);
            var usersByUserName = request.UserName == null
                ? null
                : await context.Users
                    .Where(applicationUser => applicationUser.UserName.Contains(request.UserName) &&
                                              applicationUser.UserName != currentUser.UserName
                    )
                    .ToListAsync();
            return new SearchUserResponse
            {
                UsersByUserName = usersByUserName?.Select(user => user.Serialize())
            };
        }

        [HttpGet("current")]
        public async Task<ActionResult<UserDto>> GetCurrent()
        {
            var userId = userManager.GetUserId(User);
            var user = await context.Users
                .AsSingleQuery()
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