using BC.PAYMENT.API.Models;

namespace BC.PAYMENT.API.Helper;

public static class GlobalResponse
{
    public static ApiResponse<T> SuccessResponse<T>(T  data,string message)
    {
        return ApiResponse<T>.Builder()
            .WithMessage(message).WithSuccess(true).WithResult(data)
            .WithStatusCode(StatusCodes.Status200OK)
            .Build();
    }
}