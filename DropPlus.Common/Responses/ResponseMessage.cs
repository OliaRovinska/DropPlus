using DropPlus.Common.Enums;

namespace DropPlus.Common.Responses
{
    public class ResponseMessage
    {
        public bool IsSuccess { get; set; }

        public StatusCodeEnum StatusCode { get; set; }

        public string Message { get; set; }

        public object Payload { get; set; }

        public ResponseMessage(bool isSuccess, StatusCodeEnum statusCode, string message = null, object payload = null)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Message = message;
            Payload = payload;
        }
    }
}