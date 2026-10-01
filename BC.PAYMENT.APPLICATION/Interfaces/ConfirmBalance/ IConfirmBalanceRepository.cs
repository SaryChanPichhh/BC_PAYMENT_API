using BC.PAYMENT.CORE.Contracts.Request.ConfirmBalance;
using BC.PAYMENT.CORE.Contracts.Response.ConfirmBalance;
using BC.PAYMENT.CORE.Entities;
using ConfirmBalanceAccountReceivable =
    BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.AccountReceivable.ConfirmAccountReceivableModel;

namespace BC.PAYMENT.APPLICATION.Interfaces.ConfirmBalance;

public interface IConfirmBalanceRepository
{
    Task<int> AddConfirmAccountReceivable(DtConfirmAccountReceivable req);
    Task<int> DeleteConfirmAccountReceivable(int id);
    Task<int> UpdateConfirmAccountReceivable(DtConfirmAccountReceivable req);
    Task<List<ConfirmBalanceResponse>> GetConfirmAccountReceivable(string dbCode);

    Task<List<DtConfirmAccountReceivableDetailResponse>> GetConfirmBalanceDetailsAsync(
        string dbCode, int confirmBalanceId);

    Task<int> UpdateConfirmBalanceDetailsAsync(
        DtConfirmAccountReceivableDetail req);

    Task<int> AddConfirmBalanceDetailsAsync(
        List<DtConfirmAccountReceivableDetail> requests);
}