using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Demo.Identity.Authentication.Models
{
    public class TokenIssuerOptions
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public TimeSpan AuthTokenValidFor { get; set; }

        public TimeSpan RefreshTokenValidFor { get; set; }

        public SigningCredentials AuthTokenSigningCredentials { get; set; }

        public SigningCredentials RefreshTokenSigningCredentials { get; set; }

        public DateTime AuthTokenExpiration => IssuedAt.Add(AuthTokenValidFor);

        public DateTime RefreshTokenExpiration => IssuedAt.Add(RefreshTokenValidFor);

        public DateTime NotBefore => DateTime.UtcNow;

        public DateTime IssuedAt => DateTime.UtcNow;

        public Func<Task<string>> JtiGenerator =>
          () => Task.FromResult(Guid.NewGuid().ToString());
    }
}