namespace DropPlus.Common.Enums
{
    public enum StatusCodeEnum
    {
        Ok = 200,               //Response to a successful GET, PUT, PATCH or DELETE.
        Created = 201,          //Response to a POST that results in a creation.
        NoContent = 204,        //Response to a successful request that won't be returning a body (like a DELETE request).
        NotModified = 304,      //Response has not been modified since the previous transmission.
        BadRequest = 400,       //Malformed request; request body validation errors.
        Unauthorized = 401,     //When no or invalid authentication details are provided.
        PaymentRequired = 402,  //When payment check has failed
        Forbidden = 403,        //When authentication succeeded but authenticated user doesn't have access to the resource.
        NotFound = 404,         //When a non-existent resource is requested.
        MethodNotAllowed = 405, //Method not allowed.
        NotAcceptable = 406,    //Could not satisfy the request Accept header.
        Conflict = 409,         //When the request could not be completed due to a conflict with the current state of the resource.
        ServerError = 500       //Something went wrong on the API end.
    }
}