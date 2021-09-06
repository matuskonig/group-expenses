using System.Collections.Generic;

namespace Entities.Dto.UserDto
{
    public class SearchUserResponse
    {
        public IEnumerable<AuthDto.UserDto> UsersByUserName { get; set; }
    }
}