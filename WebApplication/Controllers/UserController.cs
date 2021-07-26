using System.Threading.Tasks;
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

        [HttpGet("addFriend/{id}")]
        public async Task<ActionResult> SendFriendRequest(string id)
        {
            /*var user = await userManager.GetUserAsync(User);
            var other = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (other == null)
                return BadRequest();
            var relation = new UserRelation {Since = DateTime.Now, From = user, To = other};
            user.SendRequests.Add(relation);
            other.ReceivedRequests.Add(relation);
            await context.SaveChangesAsync();*/
            return Ok();
        }
        
        /*public ActionResult AcceptFriendRequest()
        {
            throw new NotImplementedException();
        }*/
        
        /*public ActionResult RejectFriendRequest()
        {
            throw new NotImplementedException();
        }*/

        /*[HttpGet("user/{id}")]*/
        /*public ActionResult<UserDto> GetFriend(Guid id)
        {
            throw new NotImplementedException();
        }*/
    }
}