using BC.PAYMENT.CORE.Contracts.Request.Expense;
using BC.PAYMENT.CORE.Contracts.Response.Expense;
using BC.PAYMENT.CORE.Entities;

namespace BC.PAYMENT.APPLICATION.Interfaces.Expense;

public interface IExpenseRepository
{
    Task<int> CreatePaymentExpense(PaymentInvoiceExpense expenseModel);
    Task<List<BcPaymentDetailResponse>> LoadBcPaymentDetailAsync(string dbCode, string deliveryId, DateTime date);
    Task<List<BcPaymentDetailResponse>> LoadBcPaymentDetailAsync(string dbCode, DateTime fromDate, DateTime toDate);

    Task<List<BcPaymentDetailResponse>> LoadBcPaymentDetailWithPaidAsync(string dbCode, DateTime fromDate,
        DateTime toDate);

    Task<List<BcPaymentDetailResponse>> LoadBcPaymentDetailAsync(string dbCode, int month, int year);
    Task<int> UpdateBcPaymentDetailAsync(UpdateBcPaymentDetailRequest request);
}