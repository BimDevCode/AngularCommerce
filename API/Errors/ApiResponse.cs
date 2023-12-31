namespace API.Errors
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }
        
        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "A bad request",
                401 => "Authorized, you are not",
                404 => "Resource found, it was not",
                500 => "Errors are the part to the dark side. Errors lead to anger. Anger leads to hate. Hates leads to career changes",
                _ => "Unhandled error"
            };
        }

        public int StatusCode {get;set;}
        public string Message {get;set;}
    }
}