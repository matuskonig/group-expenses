using System;

namespace Entities.Dto.GroupDto
{
    public class SinglePaymentDto
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public AuthDto.UserDto Target { get; set; }
    }
}