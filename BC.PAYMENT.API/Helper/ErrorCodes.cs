namespace BC.PAYMENT.API.Helper;

public static class ErrorCodes
{
    // Internal
    public const string ValidationError = "VALIDATION_ERROR";
    public const string UnknownError = "BACKEND_ERROR";
    public const string NotFound = "NOT_FOUND";
    public const string DbError = "DB_ERROR";
    // public const string UnknownError   = "BE_SERIALIZATION_ERROR";

    // BACKEND-specific
    public const string CbsTimeout = "CBS_TIMEOUT";
    public const string CbsConnectionFailed = "CBS_CONNECTION_FAILED";
    public const string CbsRejected = "CBS_REJECTED"; // CBS returned a business rejection
    public const string CbsInvalidResponse = "CBS_INVALID_RESPONSE";
}