using System.ComponentModel.DataAnnotations;

namespace Entities.AuthDto
{
    public class UserDto
    {
        [Required] public string UserName { get; set; }
    }
}