using Demo.Identity.Authentication.Models;
using Demo.Identity.Enums;

namespace Demo.Identity.Responses
{
    public class LoginResponseMessage : ResponseMessage
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public TokenViewModel AuthToken { get; set; }

        public TokenViewModel RefreshToken { get; set; }

        public LoginResponseMessage(bool isSuccess, string message, StatusCodeEnum statusCode, string id, string userName, TokenViewModel authToken, TokenViewModel refreshToken) 
            :base(isSuccess, message, statusCode)
        {
            Id = id;
            UserName = userName;
            AuthToken = authToken;
            RefreshToken = refreshToken;
        }
    }
}