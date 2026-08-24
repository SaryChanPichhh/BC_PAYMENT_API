namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.AccountReceivable
{
    public interface IConfirmAccountReceivableRepository : IBaseRepository<ConfirmAccountReceivableModel.ConfirmBalance>
    {
        //Task<int> AddConfirmAccountReceivable(ConfirmAccountReceivableModel.ConfirmBalance model);
        //Task<List<ConfirmAccountReceivableModel.ConfirmBalance>> GetConfirmAccountReceivable(string dbCode);
        Task<List<ConfirmAccountReceivableModel.ConfirmBalanceDetails>> GetConfirmBalanceAccountReceivableDetails(
            string dbCode, int confirmAccountReceivableId);

        Task<int> UpdateConfirmBalanceAccountReceivableDetails(string dbCode,
            ConfirmAccountReceivableModel.ConfirmBalanceDetails balanceDetails);

        Task<int> AddConfirmAccountReceivableDetails(int confirmAccountReceivableId,
            List<ConfirmAccountReceivableModel.ConfirmBalanceDetails> confirmBalanceDetails);
    }
}
