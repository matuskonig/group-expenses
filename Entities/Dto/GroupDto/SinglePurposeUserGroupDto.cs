using System;
using System.Collections.Generic;

namespace Entities.Dto.GroupDto
{
    public class SinglePurposeUserGroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<AuthDto.UserDto> GroupUsers { get; set; }
        public IEnumerable<UnidirectionalPaymentGroupDto> GroupPayments { get; set; }
    }
}