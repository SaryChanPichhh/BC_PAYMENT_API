using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.CORE.Contracts.Request.AccountReceivable;

namespace BC.PAYMENT.API.Controllers.Ar
{
    public class AccountReceivableController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpPost]
        [Route("save-ledger")]
        public async Task<ApiResponse<int>> SaveLedgerAsync([FromBody] SiLedgerRequest request, [FromQuery] bool isAccountsReceivableCompleted = false)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var affectedRow = await unitOfWork.AccountReceivable.InsertAccountReceivable(request, isAccountsReceivableCompleted);
                if (affectedRow > 0)
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("Ledger saved successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithSuccess(true)
                        .WithResult(affectedRow)
                        .Build();
                }
                return ApiResponse<int>.Builder()
                    .WithMessage("Ledger saved unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .WithResult(affectedRow)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }
    }
}