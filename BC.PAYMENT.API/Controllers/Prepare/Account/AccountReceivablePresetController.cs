using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Prepare.Account;
using BC.PAYMENT.CORE.Entities.Prepare.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Prepare.Account
{
    public class AccountReceivablePresetController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountReceivablePresetController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("")]
        public async Task<ApiResponse<AccountReceivableDto>> AddNewAccountReceivablePresetAsync([FromBody] AccountReceivableDto model)
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            var accountReceivable = new ApiResponse<AccountReceivableDto>();
            try
            {
                var accountReceivableModel = new AccountReceivablePresetModel()
                {
                    DbCode = claim.DbCode,
                    CreditDebitType = model.CreditDebitType,
                    AccountCode = model.AccountCode,
                    Description = model.Description,
                    Field1 = model.Field1,
                    Field2 = model.Field2,
                    Field3 = model.Field3,
                    Field4 = model.Field4,
                    Field5 = model.Field5,
                    Field6 = model.Field6,
                    Field7 = model.Field7,
                    Field8 = model.Field8,
                    Field9 = model.Field9,
                    CreatedBy = claim.Username,
                    CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                var affectedRows = await _unitOfWork.AccountReceivablePresets.AddNewAsync(accountReceivableModel);
                if (affectedRows > 0)
                {
                    accountReceivable.Message = "Account Receivable Preset added successfully";
                    accountReceivable.Success = true;
                    accountReceivable.StatusCode = (int)HttpStatusCode.OK;
                    accountReceivable.Result = model;
                }
                else
                {
                    accountReceivable.Message = "Account Receivable Preset added unsuccessfully";
                    accountReceivable.StatusCode = (int)HttpStatusCode.BadRequest;
                    accountReceivable.Result = model;
                }
            }catch(SqlException ex)
            {
                accountReceivable.Message = $@"Sql Exception : {ex.Message}";
                accountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                accountReceivable.Message = $@"Error Exception : {ex.Message}";
                accountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return accountReceivable;
        }
        
        [HttpGet]
        [Route("")]
        public async Task<ApiResponse<List<AccountReceivablePresetModel>>> GetAccountReceivablePresetAsync()
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            var accountReceivable = new ApiResponse<List<AccountReceivablePresetModel>>();
            try
            {
                var execute = await _unitOfWork.AccountReceivablePresets.GetAsync(claim.DbCode!);
                if (execute.Any())
                {
                    accountReceivable.Message = "Account Receivable Preset fetched successfully";
                    accountReceivable.Success = true;
                    accountReceivable.StatusCode = (int)HttpStatusCode.OK;
                    accountReceivable.Result = execute;
                }
                else
                {
                    accountReceivable.Message = "Account Receivable Preset fetched unsuccessfully";
                    accountReceivable.StatusCode = (int)HttpStatusCode.BadRequest;
                    accountReceivable.Result = execute;
                }
            }catch(SqlException ex)
            {
                accountReceivable.Message = $@"Sql Exception : {ex.Message}";
                accountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                accountReceivable.Message = $@"Error Exception : {ex.Message}";
                accountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return accountReceivable;
        }
        
        [HttpDelete]
        [Route("")]
        public async Task<ApiResponse<AccountReceivableDeleteDto>> DeleteAccountReceivablePresetAsync(AccountReceivableDeleteDto model)
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            var accountReceivable = new ApiResponse<AccountReceivableDeleteDto>();
            try
            {
                var accountReceivableModel = new AccountReceivablePresetModel
                {
                    DbCode = claim.DbCode,
                    CreditDebitType = model.CreditDebitType,
                    AccountCode = model.AccountCode,
                };
                var affectedRow = await _unitOfWork.AccountReceivablePresets.DeleteAccountReceivableAsync(accountReceivableModel);
                if (affectedRow>1)
                {
                    accountReceivable.Message = "Account Receivable Preset deleted successfully";
                    accountReceivable.Success = true;
                    accountReceivable.StatusCode = (int)HttpStatusCode.OK;
                    accountReceivable.Result = model;
                }
                else
                {
                    accountReceivable.Message = "Account Receivable Preset deleted unsuccessfully";
                    accountReceivable.StatusCode = (int)HttpStatusCode.BadRequest;
                    accountReceivable.Result = model;
                }
            }catch(SqlException ex)
            {
                accountReceivable.Message = $@"Sql Exception : {ex.Message}";
                accountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                accountReceivable.Message = $@"Error Exception : {ex.Message}";
                accountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return accountReceivable;
        }
    }
}
