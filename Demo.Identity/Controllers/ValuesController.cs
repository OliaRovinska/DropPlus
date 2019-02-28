using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Identity.Enums;
using Demo.Identity.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Identity.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ValuesController : ControllerBase
    {
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Hiring manager")]
        [ActionName("Test Hiring manager")]
        public ResponseMessage TestHiringManager()
        {
            return new ResponseMessage(true, "authorized", StatusCodeEnum.Ok);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [ActionName("Test Admin")]
        public ResponseMessage TestAdmin()
        {
            return new ResponseMessage(true, "authorized", StatusCodeEnum.Ok);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [ActionName("Test User")]
        public ResponseMessage TestUser()
        {
            return new ResponseMessage(true, "authorized", StatusCodeEnum.Ok);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("Test")]
        public ResponseMessage Test()
        {
            return new ResponseMessage(true, "authorized", StatusCodeEnum.Ok);
        }
    }
}
