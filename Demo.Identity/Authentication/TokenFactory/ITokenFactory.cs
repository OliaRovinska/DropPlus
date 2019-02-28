using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Demo.Identity.Authentication.TokenFactory
{
    public interface ITokenFactory
    {
        Task<string> GenerateAuthToken(string userName, ClaimsIdentity identity, IList<string> role);
        Task<string> GenerateRefreshToken(string userName, ClaimsIdentity identity, IList<string> role);
        ClaimsIdentity GenerateClaimsIdentity(string userName, string id);
    }
}