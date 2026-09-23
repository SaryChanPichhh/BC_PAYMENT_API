
using BC.PAYMENT.API.Models.ExpenseTypes;
namespace BC.PAYMENT.API.Controllers
{
    [Helper.Authorize]
    public class ExpenseTypeController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpGet("")]
        public async Task<ApiResponse<List<ExpenseTypeResponse>>> GetExpenseTypes()
        {
            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);

                var data = await unitOfWork.ExpenseTypes.GetExpenseTypes(claim.DbCode!);
                
                var responses = data.Select(x => new ExpenseTypeResponse
                {
                    ExpenseId = x.ExpenseId,
                    DbCode = x.DbCode,
                    ExpenseName = x.ExpenseName,
                    Status = x.Status,
                    CreatedBy = x.UserCreated,
                    CreatedAt = x.CreatedDate
                }).ToList();

                return ApiResponse<List<ExpenseTypeResponse>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Expense Types fetched successfully.")
                    .WithResult(responses)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ExpenseTypeResponse>>(ex.Message);
            }
        }

        [HttpPost("")]
        public async Task<ApiResponse<ExpenseTypeResponse>> CreateExpenseType([FromBody] CreateExpenseTypeRequest request)
        {
            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);

                var expenseType = new ExpenseType
                {
                    DbCode = request.DbCode ?? claim.DbCode,
                    ExpenseName = request.ExpenseName,
                    Status = request.Status ?? true,
                    UserCreated = claim.Username,
                    CreatedDate = DateTime.Now
                };

                var success = await unitOfWork.ExpenseTypes.CreateExpenseType(expenseType);
                if (!success)
                {
                    return ApiResponse<ExpenseTypeResponse>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Failed to create expense type.")
                        .Build();
                }

                var response = new ExpenseTypeResponse
                {
                    ExpenseId = expenseType.ExpenseId,
                    DbCode = expenseType.DbCode,
                    ExpenseName = expenseType.ExpenseName,
                    Status = expenseType.Status,
                    CreatedBy = expenseType.UserCreated,
                    CreatedAt = expenseType.CreatedDate
                };

                return ApiResponse<ExpenseTypeResponse>.Builder()
                    .WithStatusCode((int)HttpStatusCode.Created)
                    .WithMessage("Expense type created successfully.")
                    .WithResult(response)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<ExpenseTypeResponse>(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<ExpenseTypeResponse>> UpdateExpenseType(string id, [FromBody] UpdateExpenseTypeRequest request)
        {
            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);

                var expenseType = new ExpenseType
                {
                    ExpenseId = id,
                    DbCode = request.DbCode ?? claim.DbCode,
                    ExpenseName = request.ExpenseName,
                    Status = request.Status
                };

                var success = await unitOfWork.ExpenseTypes.UpdateExpenseType(expenseType);
                if (!success)
                {
                    return ApiResponse<ExpenseTypeResponse>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Failed to update expense type or not found.")
                        .Build();
                }

                var response = new ExpenseTypeResponse
                {
                    ExpenseId = expenseType.ExpenseId,
                    DbCode = expenseType.DbCode,
                    ExpenseName = expenseType.ExpenseName,
                    Status = expenseType.Status,
                    // UserCreated and CreatedDate are not updated, so we might return them empty or fetch them first.
                };

                return ApiResponse<ExpenseTypeResponse>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Expense type updated successfully.")
                    .WithResult(response)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<ExpenseTypeResponse>(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteExpenseType(string id)
        {
            try
            {
                var success = await unitOfWork.ExpenseTypes.DeleteExpenseType(id);
                if (!success)
                {
                    return ApiResponse<bool>.Builder()
                        .WithStatusCode(StatusCodes.Status404NotFound)
                        .WithMessage("Expense type not found or failed to delete.")
                        .WithResult(false)
                        .Build();
                }

                return ApiResponse<bool>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Expense type deleted successfully.")
                    .WithResult(true)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
            }
        }
    }
}
