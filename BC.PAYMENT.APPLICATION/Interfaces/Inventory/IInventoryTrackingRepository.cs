namespace BC.PAYMENT.APPLICATION.Interfaces.Inventory;

public interface IInventoryTrackingRepository
{
    #region Inventory Tracking

    Task<List<InventoryTrackingWarehouseModel>> GetInventoryTrackingWarehouseListByDbCodeAsync(string dbCode);
    Task<bool> ExistInventoryTrackingWarehouseByDbCodeAndNameAsync(string dbCode, string name);
    Task<int> UpdateInventoryTrackingAsync(InventoryTrackingWarehouseModel model);
    Task<int> DeleteInventoryTrackingAsync(string dbCode, string Id);
    Task<int> AddInventoryTrackingAsync(InventoryTrackingWarehouseModel model);

    #endregion

    #region Inventory Warehouse Data

    Task<List<InventoryTrackingWarehouseDataModel>> GetInventoryTrackingWarehouseDataListByDbCodeAndWarehouseAsync(
        string dbCode, string warehouse, InventoryTrackingTypes inventoryTrackingTypes);

    Task<int> AddWarehouseData(InventoryTrackingWarehouseDataModel model);
    Task<int> UpdateWarehouseData(InventoryTrackingWarehouseDataModel model);
    Task<int> DeleteWarehouseData(string dbCode, string id);

    #endregion

    #region Summary Inventory Tracking

    Task<List<InventoryTrackingWarehouseDataModel>> GetSummaryInventoryTrackingByDbCodeAsync(string dbCode);

    Task<List<InventoryTrackingWarehouseDataModel>> GetSummaryInventoryTrackingByDbCodeAndDateAsync(string dbCode,
        DateTime dateTime);

    Task<string> ClosingTrackingInventoryAsync(List<InventoryTrackingWarehouseDataModel> list);

    #endregion
}