using BC.PAYMENT.API.Models;
using BC.PAYMENT.CORE.DTO.Prepare.Account;

namespace BC.PAYMENT.API.Helper;

public static class GlobalExceptionHandler
{
    public static ApiResponse<T> ValidateSchemaError<T>(object validationResult, string errorMessage)
    {
        return ApiResponse<T>.Builder().WithErrors(ErrorCodes.ValidationError).WithSuccess(false)
            .WithErrors(validationResult).WithMessage(errorMessage)
            .WithStatusCode(StatusCodes.Status400BadRequest)
            .Build();
    }

    public static ApiResponse<T> ExceptionError<T>(string errorMessage)
    {
        return ApiResponse<T>.Builder()
            .WithErrors(ErrorCodes.UnknownError)
            .WithSuccess(false)
            .WithMessage(errorMessage)
            .WithStatusCode(StatusCodes.Status500InternalServerError)
            .Build();
    }
}