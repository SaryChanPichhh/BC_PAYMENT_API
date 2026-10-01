using BC.PAYMENT.CORE.Contracts.Request.Expense;
using BC.PAYMENT.CORE.Contracts.Response.Expense;
using BC.PAYMENT.CORE.Entities;

namespace BC.PAYMENT.APPLICATION.Interfaces.Submit;

public interface ISubmitExpenseRepository
{
    Task<int> AddNewSubmitExpense(BcSubmittedPaidDetail detail);

    Task<List<SubmitExpenseDetailResponse>> GetSubmitExpenseDetailByDateAsync(string dbCode, DateTime fromDate,
        DateTime toDate);

    Task<List<ApprovedSubmitExpenseDetailResponse>> GetApprovedExpenseDetailByDateAsync(string dbCode,
        DateTime fromDate, DateTime toDate);

    Task<int> UpdateSubmitExpenseDescriptionAsync(UpdateSubmitExpenseDescriptionRequest request);
    Task<int> DeleteSubmitExpenseAsync(int submittedId);
}