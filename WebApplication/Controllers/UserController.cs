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
        public async Task<ActionResult> SendFriendRequest(string id)
        {
            try
            {
                var user = await userManager.GetUserAsync(User);
                var other = await context.Users.FindAsync(id);
                var possibleExistingRelation = await context.FriendRequests
                    .FirstOrDefaultAsync(request =>
                        request.From == user && request.To == other && request.State != FriendRequestState.Rejected);
                if (other == null || possibleExistingRelation != null)
                    return BadRequest();

                var relation = new FriendshipStatus
                {
                    From = user,
                    To = other,
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

        [HttpGet("acceptRequest/{id:guid}")]
        public async Task<ActionResult> AcceptFriendRequest(Guid id)
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
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("rejectRequest/{id:guid}")]
        public async Task<ActionResult> RejectFriendRequest(Guid id)
        {
            try
            {
                var request = await context.FriendRequests.FindAsync(id);
                if (request is not {State: FriendRequestState.WaitingForAccept})
                    return BadRequest();
                request.State = FriendRequestState.Rejected;
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("searchUser")]
        public async Task<ActionResult<SearchUserResponse>> SearchFriendByName([FromBody] SearchUserRequest request)
        {
            var currentUser = await userManager.GetUserAsync(User);
            var usersByUserName = request.UserName == null
                ? Enumerable.Empty<ApplicationUser>()
                : await context.Users
                    .Where(applicationUser => applicationUser.UserName != currentUser.UserName &&
                                              applicationUser.UserName.Contains(request.UserName))
                    .ToListAsync();
            var userDto = usersByUserName
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                });
            return new SearchUserResponse
            {
                Users = userDto
            };
        }
    }
}