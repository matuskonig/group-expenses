using System;
using System.Diagnostics.CodeAnalysis;

namespace Entities.AuthDto
{
    public class LoginResponse
    {
        [NotNull] public string Token { get; set; }
        [NotNull] public DateTime Expiration { get; set; }
    }
}