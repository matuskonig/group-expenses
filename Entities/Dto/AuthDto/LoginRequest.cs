using System.ComponentModel.DataAnnotations;

namespace Entities.Dto.AuthDto
{
    public class LoginRequest
    {
        [Required] public string Email { get; set; }
        [Required] public string Password { get; set; }
    }
}