using DropPlus.Identity.Authentication.Models;

namespace DropPlus.Identity.Models
{
    public class LoginResponseModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public TokenViewModel AuthToken { get; set; }

        public TokenViewModel RefreshToken { get; set; }

        public LoginResponseModel(string id, string userName, TokenViewModel authToken, TokenViewModel refreshToken)
        {
            Id = id;
            UserName = userName;
            AuthToken = authToken;
            RefreshToken = refreshToken;
        }
    }
}