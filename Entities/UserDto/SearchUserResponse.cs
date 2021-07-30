using System.Collections.Generic;

namespace Entities.UserDto
{
    public class SearchUserResponse
    {
        public IEnumerable<AuthDto.UserDto> Users { get; set; }
    }
}