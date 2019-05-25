using DropPlus.Common.Enums;

namespace DropPlus.Common.Responses
{
    public class ErrorResponseMessage: ResponseMessage
    {
        public ErrorResponseMessage(StatusCodeEnum statusCode, string message = null, object payload = null)
            : base(false, statusCode, message, payload)
        {
        }
    }
}