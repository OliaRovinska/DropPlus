using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Demo.Identity.Authentication.Models;
using Demo.Identity.Authentication.TokenFactory;
using Demo.Identity.Data;
using Demo.Identity.Enums;
using Demo.Identity.Models;
using Demo.Identity.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Demo.Identity.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenFactory _tokenFactory;
        private readonly TokenIssuerOptions _tokenOptions;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenFactory tokenFactory, RoleManager<IdentityRole> roleManager, IOptions<TokenIssuerOptions> tokenOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenFactory = tokenFactory;
            _roleManager = roleManager;
            _tokenOptions = tokenOptions.Value;
        }

        [HttpPost]
        [ActionName("Register")]
        public async Task<ResponseMessage> Register([FromBody]RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseMessage(false, "Model state is invalid", StatusCodeEnum.BadRequest);
            }
            // add role
            string role = model.Role.ToString();
            if (await _roleManager.FindByNameAsync(role) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

            AppUser user = new AppUser
            {
                Email = model.Email,
                UserName = model.Email.Split('@').FirstOrDefault()
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                return new ResponseMessage(true, "User was added", StatusCodeEnum.Ok);
            }
            else
            {
                string errors = "";
                foreach (IdentityError error in result.Errors)
                {
                    errors += error.Code + ";";
                }
                return new ResponseMessage(false, errors, StatusCodeEnum.BadRequest);
            }
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<ResponseMessage> Login([FromBody]LoginViewModel credentials)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseMessage(false, "Model state is invalid", StatusCodeEnum.BadRequest);
            }

            var user = await _userManager.FindByEmailAsync(credentials.Email);

            //check if user exists
            if (user == null)
            {
                return new ResponseMessage(false, "User not found", StatusCodeEnum.BadRequest);
            }

            var identity = await GetClaimsIdentity(user.UserName, credentials.Password);

            if (identity == null)
            {
                return new ResponseMessage(false, "Invalid username or password", StatusCodeEnum.BadRequest);
            }

            var tokenResponse = new LoginResponseMessage(true, "", StatusCodeEnum.Ok, identity.Claims.Single(c => c.Type == "id").Value, 
                user.UserName, await GetAuthToken(user), await GetRefreshToken(user));

            // update user
            user.RefreshToken = tokenResponse.RefreshToken.Token;
            user.ValidUntil = tokenResponse.RefreshToken.ValidUntil;
            await _userManager.UpdateAsync(user);

            return tokenResponse;
        }

        [HttpPost]
        [ActionName("GetAuthToken")]
        public async Task<ResponseMessage> GetAuthToken([FromBody]GetAuthTokenViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseMessage(false, "Model state is invalid", StatusCodeEnum.BadRequest);
            }

            var user = await _userManager.FindByIdAsync(model.Id);

            // check if user exists
            if (user == null)
            {
                return new ResponseMessage(false, "User not found", StatusCodeEnum.BadRequest);
            }

            // check refresh token
            if (string.IsNullOrEmpty(user.RefreshToken) || user.RefreshToken != model.RefreshToken)
            {
                return new ResponseMessage(false, "Refresh token is incorrect", StatusCodeEnum.BadRequest);
            }

            if (user.ValidUntil == null || user.ValidUntil <= DateTime.UtcNow)
            {
                return new ResponseMessage(false, "Refresh token is expired", StatusCodeEnum.BadRequest);
            }

            var tokenResponse = new LoginResponseMessage(true, "Refresh token is correct", StatusCodeEnum.Ok, model.Id,
                user.UserName, await GetAuthToken(user), await GetRefreshToken(user));

            // update user
            user.RefreshToken = tokenResponse.RefreshToken.Token;
            user.ValidUntil = tokenResponse.RefreshToken.ValidUntil;
            await _userManager.UpdateAsync(user);

            return tokenResponse;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("Logout")]
        public async Task<ResponseMessage> Logout()
        {
            if (!ModelState.IsValid)
            {
                return new ResponseMessage(false, "Model state is invalid", StatusCodeEnum.BadRequest);
            }

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return new ResponseMessage(false, "Token is invalid. Can't take user's claims", StatusCodeEnum.BadRequest);
            }

            var user = await _userManager.FindByIdAsync(identity.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value);
            // check if user exists
            if (user == null)
            {
                return new ResponseMessage(false, "User not found", StatusCodeEnum.BadRequest);
            }

            // update user
            user.RefreshToken = null;
            user.ValidUntil = null;
            await _userManager.UpdateAsync(user);

            await _signInManager.SignOutAsync();

            return new ResponseMessage(true, "User was logged out", StatusCodeEnum.Ok);
        }

        private async Task<TokenViewModel> GetAuthToken(AppUser user)
        {
            var identity = _tokenFactory.GenerateClaimsIdentity(user.UserName, user.Id);
            var roles = await _userManager.GetRolesAsync(user);
            var response = new TokenViewModel
            {
                Token = await _tokenFactory.GenerateAuthToken(user.UserName, identity, roles),
                ValidUntil = _tokenOptions.IssuedAt.Add(_tokenOptions.AuthTokenValidFor)
            };
            return response;
        }

        private async Task<TokenViewModel> GetRefreshToken(AppUser user)
        {
            var identity = _tokenFactory.GenerateClaimsIdentity(user.UserName, user.Id);
            var roles = await _userManager.GetRolesAsync(user);
            var response = new TokenViewModel
            {
                Token = await _tokenFactory.GenerateRefreshToken(user.UserName, identity, roles),
                ValidUntil = _tokenOptions.IssuedAt.Add(_tokenOptions.RefreshTokenValidFor)
            };
            return response;
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return await Task.FromResult<ClaimsIdentity>(null);

            // get the user to verifty
            var userToVerify = await _userManager.FindByNameAsync(userName);

            if (userToVerify == null) return await Task.FromResult<ClaimsIdentity>(null);

            // check the credentials
            if (await _userManager.CheckPasswordAsync(userToVerify, password))
            {
                return await Task.FromResult(_tokenFactory.GenerateClaimsIdentity(userName, userToVerify.Id));
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }
    }
}
