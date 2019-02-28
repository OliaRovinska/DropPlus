using System;
using Microsoft.AspNetCore.Identity;

namespace Demo.Identity.Data
{
    public class AppUser: IdentityUser
    {
        public string RefreshToken { get; set; }

        public DateTime? ValidUntil { get; set; }
    }
}