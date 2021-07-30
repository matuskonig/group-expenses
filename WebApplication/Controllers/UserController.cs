using System;
using System.Linq;
using System.Threading.Tasks;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
                var relation = new FriendshipStatus
                    {From = user, To = other, Created = DateTime.Now, State = FriendRequestState.WaitingForAccept};
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
                var friendRequest = await context.FriendRequests.FindAsync(id);
                friendRequest.State = FriendRequestState.Accepted;
                var inverseRelation = new FriendshipStatus
                {
                    From = friendRequest.To, To = friendRequest.From, Created = DateTime.Now,
                    State = FriendRequestState.Accepted
                };
                await context.FriendRequests.AddAsync(inverseRelation);
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
                request.State = FriendRequestState.Rejected;
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}