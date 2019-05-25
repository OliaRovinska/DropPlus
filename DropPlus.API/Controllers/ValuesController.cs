using DropPlus.Common.Authentication;
using DropPlus.Common.Enums;
using DropPlus.Common.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DropPlus.API.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ValuesController : ControllerBase
    {
        [HttpPost]
        [MyAuthorize(RoleEnum.HiringManager)]
        [ActionName("Test Hiring manager")]
        public ResponseMessage TestHiringManager()
        {
            return new SuccessResponseMessage(StatusCodeEnum.Ok);
        }

        [HttpPost]
        [MyAuthorize(RoleEnum.Admin)]
        [ActionName("Test Admin")]
        public ResponseMessage TestAdmin()
        {
            return new SuccessResponseMessage(StatusCodeEnum.Ok);
        }

        [HttpPost]
        [MyAuthorize(RoleEnum.User)]
        [ActionName("Test User")]
        public ResponseMessage TestUser()
        {
            return new SuccessResponseMessage(StatusCodeEnum.Ok);
        }

        [HttpPost]
        [MyAuthorize]
        [ActionName("Test")]
        public ResponseMessage Test()
        {
            return new SuccessResponseMessage(StatusCodeEnum.Ok);
        }
    }
}