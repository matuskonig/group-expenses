namespace Entities.Dto.GroupDto
{
    public struct PaymentRecordDto
    {
        public AuthDto.UserDto PaymentBy { get; set; }
        public AuthDto.UserDto PaymentFor { get; set; }
        public decimal Price { get; set; }
    }
}