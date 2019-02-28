using System;
using System.Text;
using Demo.Identity.Authentication.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Demo.Identity.Helpers
{
    public static class AuthorizationServiceExtensions
    {
        public static IServiceCollection AddBearerAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            // Get options from app settings
            var configurationOptions = configuration.GetSection(nameof(TokenIssuerOptions));

            // Configure TokenIssuerOptions object
            var authTokenSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configurationOptions["AuthTokenSecretKey"]));
            var refreshTokenSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configurationOptions["RefreshTokenSecretKey"]));
            services.Configure<TokenIssuerOptions>(options =>
            {
                options.Issuer = configurationOptions[nameof(TokenIssuerOptions.Issuer)];
                options.Audience = configurationOptions[nameof(TokenIssuerOptions.Audience)];
                options.AuthTokenValidFor = TimeSpan.FromMinutes(Convert.ToInt32(configurationOptions[nameof(TokenIssuerOptions.AuthTokenValidFor)]));
                options.RefreshTokenValidFor = TimeSpan.FromMinutes(Convert.ToInt32(configurationOptions[nameof(TokenIssuerOptions.RefreshTokenValidFor)]));
                options.AuthTokenSigningCredentials = new SigningCredentials(authTokenSigningKey, SecurityAlgorithms.HmacSha256);
                options.RefreshTokenSigningCredentials = new SigningCredentials(refreshTokenSigningKey, SecurityAlgorithms.HmacSha256);
            });

            // Set up Authentication
            services.AddAuthentication()
                .AddCookie(config => { config.SlidingExpiration = true; })
                .AddJwtBearer(configureOptions =>
                {
                    configureOptions.RequireHttpsMetadata = false;
                    configureOptions.SaveToken = true;
                    configureOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configurationOptions[nameof(TokenIssuerOptions.Issuer)],

                        ValidateAudience = true,
                        ValidAudience = configurationOptions[nameof(TokenIssuerOptions.Audience)],

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = authTokenSigningKey,

                        RequireExpirationTime = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            return services;
        }
    }
}