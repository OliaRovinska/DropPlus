using System;
using System.Linq;
using DropPlus.Common.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace DropPlus.Common.Authentication
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class MyAuthorizeAttribute : AuthorizeAttribute
    {
        public MyAuthorizeAttribute(params RoleEnum[] roles)
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;

            if (roles != null && roles.Length != 0)
            {
                Roles = string.Join(",", roles.Select(r => Enum.GetName(r.GetType(), r)));
            }
        }
    }
}