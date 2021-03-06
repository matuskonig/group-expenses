using System;
using System.Collections.Generic;

namespace Entities.Dto.GroupDto
{
    public class UnidirectionalPaymentGroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public AuthDto.UserDto PaymentBy { get; set; }
        public IEnumerable<SinglePaymentDto> PaymentTargets { get; set; }
    }
}