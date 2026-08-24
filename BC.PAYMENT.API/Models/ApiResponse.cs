namespace BC.PAYMENT.API.Models
{
    public class ApiResponse<T>
    {
      
        // Indicates if the operation was successful
        public bool Success { get; set; } = false;

        // A human-readable message providing more context about the response
        public string? Message { get; set; }

        // The main result payload
        public T? Result { get; set; }

        // HTTP status code for the response (e.g., 200, 400, 401)
        public int StatusCode { get; set; }

        // Additional metadata that might be useful (e.g., pagination info, extra data)
        public Dictionary<string, object>? Metadata { get; set; } = new();

        // A list of error messages (useful for validation errors)
        public object Errors { get; set; } 

        // Server timestamp for when the response was created
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Constructor to initialize default values

        public static ApiResponseBuilder Builder()
        {
            return new ApiResponseBuilder();
        }

        public class ApiResponseBuilder
        {
            private readonly ApiResponse<T> _response = new();
            public ApiResponseBuilder Success()
            {
                _response.Success = true;
                return this;
            }
            public ApiResponseBuilder Failure()
            {
                _response.Success = false;
                return this;
            }
            public ApiResponseBuilder WithSuccess(bool isSuccess)
            {
                _response.Success = isSuccess;
                return this;
            }
            public ApiResponseBuilder WithMessage(string message)
            {
                _response.Message = message;
                return this;
            }
            public ApiResponseBuilder WithResult(T result)
            {
                _response.Result = result;
                return this;
            }
            public ApiResponseBuilder WithStatusCode(int statusCode)
            {
                _response.StatusCode = statusCode;
                _response.Success = (statusCode >= 200 && statusCode < 300);
                return this;
            }
            public ApiResponseBuilder WithErrors(object errors)
            {
                _response.Errors = errors;
                return this;
            }
            public ApiResponseBuilder AddMetadata(string key, object value)
            {
                _response.Metadata[key] = value;
                return this;
            }
            public ApiResponse<T> Build()
            {
                return _response;
            }
        }
    }

}
