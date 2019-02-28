using System;

namespace Demo.Identity.Authentication.Models
{
    public class TokenViewModel
    {
        public string Token { get; set; }

        public DateTime ValidUntil { get; set; }
    }
}