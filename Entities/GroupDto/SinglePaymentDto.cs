using System;

namespace Entities.GroupDto
{
    public class SinglePaymentDto
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public AuthDto.UserDto Target { get; set; }
    }
}