using Demo.Identity.Enums;

namespace Demo.Identity.Responses
{
    public class ResponseMessage
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public StatusCodeEnum StatusCode { get; set; }

        public ResponseMessage(bool isSuccess, string message, StatusCodeEnum statusCode)
        {
            IsSuccess = isSuccess;
            Message = message;
            StatusCode = statusCode;
        }
    }
}