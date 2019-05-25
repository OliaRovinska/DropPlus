using DropPlus.Common.Enums;

namespace DropPlus.Common.Responses
{
    public class SuccessResponseMessage : ResponseMessage
    {
        public SuccessResponseMessage(StatusCodeEnum statusCode, string message = null, object payload = null)
            : base(true, statusCode, message, payload)
        {
        }
    }
}