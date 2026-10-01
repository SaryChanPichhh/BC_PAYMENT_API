namespace BC.PAYMENT.APPLICATION.Interfaces.Inventory;

public interface IInventoryReportRepository
{
    Task<List<InventoryReportModel>> GetAllStatusProductsSaleByDateAsync(
        InventoryReportRequestDto.RequestByDateDto model);

    Task<List<InventoryReportModel>> GetAllStatusProductsSaleByPeriodAsync(
        InventoryReportRequestDto.RequestByPeriodDto model);

    Task<List<InventoryReportModel>> GetAllStatusProductsSaleByDateWithOutSaleFixAsync(
        InventoryReportRequestDto.RequestByDateDto model);

    Task<List<InventoryReportModel>> GetAllStatusProductsSaleByPeriodWithOutSaleFixAsync(
        InventoryReportRequestDto.RequestByPeriodDto model);

    Task<int> AddWarehouseData(List<InventoryTrackingWarehouseDataModel> model);


    #region InventoryReceive

    Task<List<InventoryReceiveModel>> GetInventoryReceiveAsync(DateTime fromDate, DateTime toDate);

    #endregion
}