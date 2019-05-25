using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DropPlus.Common.Authentication;
using DropPlus.Common.Authentication.Models;
using DropPlus.Common.Enums;
using DropPlus.Common.Responses;
using DropPlus.DAL.Entities;
using DropPlus.Identity.Authentication.Models;
using DropPlus.Identity.Authentication.TokenFactory;
using DropPlus.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DropPlus.Identity.Controllers
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
                return new ErrorResponseMessage(StatusCodeEnum.BadRequest, "Model state is invalid");
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
                return new SuccessResponseMessage(StatusCodeEnum.Ok, "User was added");
            }
            else
            {
                string errors = "";
                foreach (IdentityError error in result.Errors)
                {
                    errors += error.Code + ";";
                }
                return new ErrorResponseMessage(StatusCodeEnum.BadRequest, errors);
            }
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<ResponseMessage> Login([FromBody]LoginViewModel credentials)
        {
            if (!ModelState.IsValid)
            {
                return new ErrorResponseMessage(StatusCodeEnum.BadRequest, "Model state is invalid");
            }

            var user = await _userManager.FindByEmailAsync(credentials.Email);

            //check if user exists
            if (user == null)
            {
                return new ErrorResponseMessage(StatusCodeEnum.BadRequest, "User not found");
            }

            var identity = await GetClaimsIdentity(user.UserName, credentials.Password);

            if (identity == null)
            {
                return new ErrorResponseMessage(StatusCodeEnum.BadRequest, "Invalid username or password");
            }

            var result = new LoginResponseModel(identity.Claims.Single(c => c.Type == "id").Value, user.UserName, await GetAuthToken(user), await GetRefreshToken(user));

            // update user
            user.RefreshToken = result.RefreshToken.Token;
            user.RefreshTokenValidUntil = result.RefreshToken.ValidUntil;
            await _userManager.UpdateAsync(user);

            return new SuccessResponseMessage(StatusCodeEnum.Ok, "", result);
        }

        [HttpPost]
        [ActionName("GetAuthToken")]
        public async Task<ResponseMessage> GetAuthToken([FromBody]GetAuthTokenViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new ErrorResponseMessage(StatusCodeEnum.BadRequest, "Model state is invalid");
            }

            var user = await _userManager.FindByIdAsync(model.Id);

            // check if user exists
            if (user == null)
            {
                return new ErrorResponseMessage(StatusCodeEnum.BadRequest, "User not found");
            }

            // check refresh token
            if (string.IsNullOrEmpty(user.RefreshToken) || user.RefreshToken != model.RefreshToken)
            {
                return new ErrorResponseMessage(StatusCodeEnum.BadRequest, "Refresh token is incorrect");
            }

            if (user.RefreshTokenValidUntil == null || user.RefreshTokenValidUntil <= DateTime.UtcNow)
            {
                return new ErrorResponseMessage(StatusCodeEnum.BadRequest, "Refresh token is expired");
            }

            var result = new LoginResponseModel(model.Id, user.UserName, await GetAuthToken(user), await GetRefreshToken(user));

            // update user
            user.RefreshToken = result.RefreshToken.Token;
            user.RefreshTokenValidUntil = result.RefreshToken.ValidUntil;
            await _userManager.UpdateAsync(user);

            return new SuccessResponseMessage(StatusCodeEnum.Ok, "Refresh token is correct", result);
        }

        [HttpPost]
        [MyAuthorize]
        [ActionName("Logout")]
        public async Task<ResponseMessage> Logout()
        {
            if (!ModelState.IsValid)
            {
                return new ErrorResponseMessage(StatusCodeEnum.BadRequest, "Model state is invalid");
            }

            if (!(HttpContext.User.Identity is ClaimsIdentity identity))
            {
                return new ErrorResponseMessage(StatusCodeEnum.BadRequest, "Token is invalid. Can't take user's claims");
            }

            var user = await _userManager.FindByIdAsync(identity.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value);
            // check if user exists
            if (user == null)
            {
                return new ErrorResponseMessage(StatusCodeEnum.BadRequest, "User not found");
            }

            // update user
            user.RefreshToken = null;
            user.RefreshTokenValidUntil = null;
            await _userManager.UpdateAsync(user);

            await _signInManager.SignOutAsync();

            return new SuccessResponseMessage(StatusCodeEnum.Ok, "User was logged out");
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

            // get the user to verify
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
