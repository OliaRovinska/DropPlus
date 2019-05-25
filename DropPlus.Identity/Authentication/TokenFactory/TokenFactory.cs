using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using DropPlus.Common.Authentication.Models;
using Microsoft.Extensions.Options;

namespace DropPlus.Identity.Authentication.TokenFactory
{
    public class TokenFactory : ITokenFactory
    {
        private const string Role = "User";
        private const string Id = "id";
        private const string ApiAccess = "api_access";
        private readonly TokenIssuerOptions _tokenOptions;

        public TokenFactory(IOptions<TokenIssuerOptions> tokenOptions)
        {
            _tokenOptions = tokenOptions.Value;
            ThrowIfInvalidOptions(_tokenOptions);
        }

        public async Task<string> GenerateAuthToken(string userName, ClaimsIdentity identity, IList<string> roles)
        {
            var claims = await GetClaims(userName, identity, roles);

            // Create the JWT security token and encode it.
            var token = new JwtSecurityToken(
                issuer: _tokenOptions.Issuer,
                audience: _tokenOptions.Audience,
                claims: claims,
                notBefore: _tokenOptions.NotBefore,
                expires: _tokenOptions.AuthTokenExpiration,
                signingCredentials: _tokenOptions.AuthTokenSigningCredentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshToken(string userName, ClaimsIdentity identity, IList<string> roles)
        {
            var claims = await GetClaims(userName, identity, roles);

            // Create the JWT security token and encode it.
            var token = new JwtSecurityToken(
                issuer: _tokenOptions.Issuer,
                audience: _tokenOptions.Audience,
                claims: claims,
                notBefore: _tokenOptions.NotBefore,
                expires: _tokenOptions.RefreshTokenExpiration,
                signingCredentials: _tokenOptions.RefreshTokenSigningCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsIdentity GenerateClaimsIdentity(string userName, string id)
        {
            return new ClaimsIdentity(new GenericIdentity(userName, "Token"), new[]
            {
                new Claim(Id, id),
                new Claim(Role, ApiAccess),
            });
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        private static void ThrowIfInvalidOptions(TokenIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.AuthTokenValidFor <= TimeSpan.Zero)
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(TokenIssuerOptions.AuthTokenValidFor));

            if (options.RefreshTokenValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(TokenIssuerOptions.RefreshTokenValidFor));
            }

            if (options.AuthTokenSigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(TokenIssuerOptions.AuthTokenSigningCredentials));
            }

            if (options.RefreshTokenSigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(TokenIssuerOptions.RefreshTokenSigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(TokenIssuerOptions.JtiGenerator));
            }
        }

        private async Task<List<Claim>> GetClaims(string userName, ClaimsIdentity identity, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, await _tokenOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_tokenOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                identity.FindFirst(Role),
                identity.FindFirst(Id)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
    }
}