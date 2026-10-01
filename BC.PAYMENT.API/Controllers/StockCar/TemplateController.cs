using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.CORE.Contracts.Request.StockCar;
using BC.PAYMENT.CORE.Contracts.Response.StockCar;
using BC.PAYMENT.CORE.Entities;

namespace BC.PAYMENT.API.Controllers.StockCar;

public class TemplateController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("")]
    public async Task<ApiResponse<List<TemplateResponse>>> GetTemplatesAsync()
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var data = await unitOfWork.Template.GetTemplatesAsync(credential?.DbCode);
            if (data.Count > 0)
                return ApiResponse<List<TemplateResponse>>.Builder()
                    .WithMessage("data fetched successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            return ApiResponse<List<TemplateResponse>>.Builder()
                .WithMessage("data fetched empty.")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<TemplateResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("by-employee/{employeeId:int}")]
    public async Task<ApiResponse<List<TemplateResponse>>> GetTemplatesByEmployeeIdAsync([FromRoute] int employeeId)
    {
        try
        {
            var data = await unitOfWork.Template.GetTemplatesByEmployeeIdAsync(employeeId);
            return ApiResponse<List<TemplateResponse>>.Builder()
                .WithMessage(data.Count > 0 ? "data fetched successfully." : "data fetched empty.")
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithResult(data)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<TemplateResponse>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("")]
    public async Task<ApiResponse<int>> AddNewTemplateAsync([FromBody] CreateTemplateRequest req)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var model = new Template
            {
                DbCode = credential.DbCode,
                CreatedBy = credential.Username,
                CreatedDate = credential.CurrectDate,
                Employee = req.EmployeeId,
                Description = req.Description
            };
            var data = await unitOfWork.Template.AddNewTemplateAsync(model);
            if (data > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("data added successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            return ApiResponse<int>.Builder()
                .WithMessage("data added unsuccessfully.")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPut]
    [Route("")]
    public async Task<ApiResponse<int>> UpdateTemplateAsync([FromBody] UpdateTemplateRequest req)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var model = new Template
            {
                Id = req.Id,
                Employee = req.EmployeeId,
                Description = req.Description
            };
            var data = await unitOfWork.Template.UpdateTemplateAsync(model);
            if (data > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("data updated successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            return ApiResponse<int>.Builder()
                .WithMessage("data updated unsuccessfully.")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<ApiResponse<int>> DeleteTemplateAsync([Required] int id)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var data = await unitOfWork.Template.DeleteTemplateAsync(id);
            if (data > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("data deleted successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            return ApiResponse<int>.Builder()
                .WithMessage("data deleted unsuccessfully.")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPut]
    [Route("disable/{id:int}")]
    public async Task<ApiResponse<int>> DisableTemplateAsync([Required] int id)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var data = await unitOfWork.SaleRepresent.DisableTemplateById(credential?.Username, id);
            if (data > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("template disabled successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            return ApiResponse<int>.Builder()
                .WithMessage("template disabled unsuccessfully.")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }
}