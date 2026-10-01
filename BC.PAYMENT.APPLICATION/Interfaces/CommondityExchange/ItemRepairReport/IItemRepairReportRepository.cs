namespace BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.ItemRepairReport;

public interface IItemRepairReportRepository
{
    Task<List<ItemRepairReceivedDto>> GetItemRepairReceivedAsync(string dbCode, DateTime fromDate, DateTime toDate);
    Task<List<ReportRepairItemInHandDto>> GetItemRepairSendToRepairerReportAsync(string dbCode);
    Task<List<ReportItemInRepairDto>> GetItemRepairingReportAsync(string dbCode);
    Task<List<ItemRepairAnalysis>> GetItemRepairReportAsync(string dbCode);
}