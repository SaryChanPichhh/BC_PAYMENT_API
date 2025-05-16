namespace BC.PAYMENT.API.Models
{
    public class ApiResponse<T>
    {
        // Indicates if the operation was successful
        public bool Success { get; set; }

        // A human-readable message providing more context about the response
        public string? Message { get; set; }

        // The main result payload
        public T? Result { get; set; }

        // HTTP status code for the response (e.g., 200, 400, 401)
        public int StatusCode { get; set; }

        // Additional metadata that might be useful (e.g., pagination info, extra data)
        public Dictionary<string, object>? Metadata { get; set; }

        // A list of error messages (useful for validation errors)
        public List<string>? Errors { get; set; }

        // Server timestamp for when the response was created
        public DateTime Timestamp { get; set; }

        // Constructor to initialize default values
        public ApiResponse()
        {
            Success = false;
            Metadata = new Dictionary<string, object>();
            Errors = new List<string>();
            Timestamp = DateTime.UtcNow;
        }
    }

}
