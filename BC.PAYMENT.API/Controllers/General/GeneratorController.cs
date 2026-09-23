using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.CORE.DTO.Generator;

namespace BC.PAYMENT.API.Controllers.General
{
    public class GeneratorController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpGet]
        [Route("sale-analysis/{customerCode}")]
        public async Task<ApiResponse<SaleAnalysisDto>> GetSaleAnalysisByCustomerCodeAsync([Required] string customerCode)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.Generators.GetSaleAnalysisByCustomerCodeAsync(customerCode, credential.DbCode!);
                if (execute != null)
                {
                    return ApiResponse<SaleAnalysisDto>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Sale analysis fetched successfully")
                        .WithSuccess(true)
                        .WithResult(execute)
                        .Build();
                }
                return ApiResponse<SaleAnalysisDto>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Sale analysis fetched unsuccessfully")
                    .WithSuccess(false)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<SaleAnalysisDto>(ex.Message);
            }
        }

        [HttpGet]
        [Route("adj-ref-code")]
        public async Task<ApiResponse<string>> GenerateAdjRefCodeAsync([Required][FromQuery] string movType, [Required][FromQuery] string recType)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.Generators.GenerateAdjRefCode(credential.DbCode!, movType, recType);
                if (!string.IsNullOrEmpty(execute))
                {
                    return ApiResponse<string>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Adjustment reference code generated successfully")
                        .WithSuccess(true)
                        .WithResult(execute)
                        .Build();
                }
                return ApiResponse<string>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Adjustment reference code generated unsuccessfully")
                    .WithSuccess(false)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<string>(ex.Message);
            }
        }

        [HttpGet]
        [Route("fix-invoice")]
        public async Task<ApiResponse<string>> GenerateFixInvoiceAsync()
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.Generators.GenerateFixInvoice(credential.DbCode!);
                if (!string.IsNullOrEmpty(execute))
                {
                    return ApiResponse<string>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Fix invoice generated successfully")
                        .WithSuccess(true)
                        .WithResult(execute)
                        .Build();
                }
                return ApiResponse<string>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Fix invoice generated unsuccessfully")
                    .WithSuccess(false)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<string>(ex.Message);
            }
        }

        [HttpPost]
        [Route("sale-order-auto-number")]
        public async Task<ApiResponse<string>> PostSaleOrderAutoNumberAsync([Required][FromQuery] string saleType)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.Generators.PostSaleOrderAutoNumberAsync(saleType, credential.DbCode!);
                if (!string.IsNullOrEmpty(execute))
                {
                    return ApiResponse<string>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Sale order auto number generated successfully")
                        .WithSuccess(true)
                        .WithResult(execute)
                        .Build();
                }
                return ApiResponse<string>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Sale order auto number generated unsuccessfully")
                    .WithSuccess(false)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<string>(ex.Message);
            }
        }

        [HttpPost]
        [Route("credit-note-auto-number")]
        public async Task<ApiResponse<string>> PostCreditNoteAutoNumberAsync([Required][FromQuery] string saleType)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.Generators.PostCreditNoteAutoNumberAsync(credential.DbCode!, saleType);
                if (!string.IsNullOrEmpty(execute))
                {
                    return ApiResponse<string>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Credit note auto number generated successfully")
                        .WithSuccess(true)
                        .WithResult(execute)
                        .Build();
                }
                return ApiResponse<string>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Credit note auto number generated unsuccessfully")
                    .WithSuccess(false)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<string>(ex.Message);
            }
        }

        [HttpGet]
        [Route("sale-code")]
        public async Task<ApiResponse<List<string>>> GetSaleCodeAsync()
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.Generators.GetSaleCodeAsync(credential.DbCode!);
                if (execute.Any())
                {
                    return ApiResponse<List<string>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Sale codes fetched successfully")
                        .WithSuccess(true)
                        .WithResult(execute)
                        .Build();
                }
                return ApiResponse<List<string>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Sale codes fetched unsuccessfully")
                    .WithSuccess(false)
                    .WithResult([])
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<string>>(ex.Message);
            }
        }
    }
}