using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.CORE.DTO.General;
using BC.PAYMENT.CORE.Entities.General;
using static BC.PAYMENT.CORE.Entities.Report.ProvincialPayment.CarPaymentModel;

namespace BC.PAYMENT.API.Controllers.Report.ProvincialPayment
{
    public class CarPaymentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public CarPaymentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Get Employee

        [HttpGet]
        [Route("getemployeehascompleted")]
        public async Task<ApiResponse<List<EmployeeDto>>> GetAllHasCompletedPayment()
        {
            try
            {
                var execute = await _unitOfWork.Employee.GetAllHasCompletedPayment();
                if (execute.Any())
                {
                    return ApiResponse<List<EmployeeDto>>.Builder()
                        .WithResult(execute.Select(x => new EmployeeDto
                        {
                            EmployeeName = x.EmployeeName,
                            EmployeeId = x.EmployeeId,
                        }).ToList())
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Employee fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<EmployeeDto>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Employee fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<EmployeeDto>>(ex.Message);
            }

        }

        #endregion

        #region Car Payment Credit Invoice

        [HttpGet]
        [Route("getcreditinvoicebyemployeeidandtemplateid/{employeeId}/{templateId}")]
        public async Task<ApiResponse<List<CarPaymentCreditInvoiceModel>>> ReportAllCreditInvoiceByEmployeeIdAndTemplateId([Required] int employeeId, [Required] int templateId)
        {
            try
            {
                var execute = await _unitOfWork.CarPayment.ReportAllCreditInvoiceByEmployeeIdAndTemplateId(employeeId, templateId);
                if (execute.Any())
                {
                    return ApiResponse<List<CarPaymentCreditInvoiceModel>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Car payment fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<CarPaymentCreditInvoiceModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Car payment fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<CarPaymentCreditInvoiceModel>>(ex.Message);
            }
        }
        [HttpGet]
        [Route("getcreditinvoicebyemployeeidbydate/{employeeId}/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<CarPaymentCreditInvoiceModel>>> ReportAllCreditInvoiceByEmployeeIdAndTemplateId([Required] int employeeId, [Required] DateTime fromDate, [Required] DateTime toDate)
        {
            try
            {
                var execute = await _unitOfWork.CarPayment.ReportAllCreditInvoiceByEmployeeIdAndDate(employeeId, fromDate, toDate);
                if (execute.Any())
                {
                    return ApiResponse<List<CarPaymentCreditInvoiceModel>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Car payment fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<CarPaymentCreditInvoiceModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Car payment fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<CarPaymentCreditInvoiceModel>>(ex.Message);
            }
        }

        #endregion 

        #region  Payment Invoice

        [HttpGet]
        [Route("getpaymentinvoicebyemployeeidandtemplateid/{employeeId}/{templateId}")]
        public async Task<ApiResponse<List<CarPaymentInvoiceModel>>> ReportAllPaymentInvoiceByEmployeeIdAndTemplateId([Required] int employeeId, [Required] int templateId)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await _unitOfWork.CarPayment.ReportAllPaymentInvoiceByEmployeeIdAndTemplateId(credential.DbCode,employeeId, templateId);
                if (execute.Any())
                {
                    return ApiResponse<List<CarPaymentInvoiceModel>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Car payment invoice fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<CarPaymentInvoiceModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Car payment invoice fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<CarPaymentInvoiceModel>>(ex.Message);
            }
        }
        [HttpGet]
        [Route("getpaymentinvoicebyemployeeidbydate/{employeeId}/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<CarPaymentInvoiceModel>>> ReportAllPaymentInvoiceByEmployeeIdAndDate([Required] int employeeId, [Required] DateTime fromDate, [Required] DateTime toDate)
        {
            try
            {
                var execute = await _unitOfWork.CarPayment.ReportAllPaymentInvoiceByEmployeeIdAndDate(employeeId, fromDate, toDate);
                if (execute.Any())
                {
                    return ApiResponse<List<CarPaymentInvoiceModel>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Car payment invoice fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<CarPaymentInvoiceModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Car payment invoice fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<CarPaymentInvoiceModel>>(ex.Message);
            }
        }

        #endregion

        #region Car Payment Expense

        [HttpGet]
        [Route("getpaymentexpensebyemployeeidandtemplateid/{employeeId}/{templateId}")]
        public async Task<ApiResponse<List<CarPaymentExpenseModel>>> ReportAllExpenseByEmployeeIdAndTemplateId([Required] int employeeId, [Required] int templateId)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await _unitOfWork.CarPayment.ReportAllExpenseByEmployeeIdAndTemplateId(employeeId, templateId);
                if (execute.Any())
                {
                    return ApiResponse<List<CarPaymentExpenseModel>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Car payment expense fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<CarPaymentExpenseModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Car payment expense fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<CarPaymentExpenseModel>>(ex.Message);
            }
        }
        [HttpGet]
        [Route("getpaymentexpensebyemployeeidbydate/{employeeId}/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<CarPaymentExpenseModel>>> ReportAllExpenseByEmployeeIdAndDate([Required] int employeeId, [Required] DateTime fromDate, [Required] DateTime toDate)
        {
            try
            {
                var execute = await _unitOfWork.CarPayment.ReportAllExpenseByEmployeeIdAndDate(employeeId, fromDate, toDate);
                if (execute.Any())
                {
                    return ApiResponse<List<CarPaymentExpenseModel>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Car payment expense fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<CarPaymentExpenseModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Car payment expense fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<CarPaymentExpenseModel>>(ex.Message);
            }
        }

        #endregion
        
        #region Car Payment Transfer Money

        [HttpGet]
        [Route("getpaymenttransferbyemployeeidandtemplateid/{employeeId}/{templateId}")]
        public async Task<ApiResponse<List<TransferMoney>>> ReportAllTransferByEmployeeIdAndTemplateId([Required] int employeeId, [Required] int templateId)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await _unitOfWork.CarPayment.ReportAllTransferByEmployeeIdAndTemplateId(credential.DbCode, employeeId, templateId);
                if (execute.Any())
                {
                    return ApiResponse<List<TransferMoney>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Car payment transfer fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<TransferMoney>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Car payment transfer fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<TransferMoney>>(ex.Message);
            }
        }
        [HttpGet]
        [Route("getpaymenttransferbyemployeeidbydate/{employeeId}/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<TransferMoney>>> ReportAllTransferByEmployeeIdAndDate([Required] int employeeId, [Required] DateTime fromDate, [Required] DateTime toDate)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await _unitOfWork.CarPayment.ReportAllTransferByEmployeeIdAndDate(credential.DbCode, employeeId, fromDate, toDate);
                if (execute.Any())
                {
                    return ApiResponse<List<TransferMoney>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Car payment transfer fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<TransferMoney>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Car payment transfer fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<TransferMoney>>(ex.Message);
            }
        }

        #endregion

    }
}
