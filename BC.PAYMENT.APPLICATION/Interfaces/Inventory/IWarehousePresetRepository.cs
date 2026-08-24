namespace BC.PAYMENT.APPLICATION.Interfaces.Inventory
{
    public interface IWarehousePresetRepository
    {
        Task<List<WarehousePresetModel>> GetWarehousePresetsAsync();
        Task<int> UpdateWarehouseAsync(WarehousePresetModel warehousePresetModel);
        Task<int> DeleteWarehouseAsync(int id);
        Task<int> CreateWarehouseAsync(WarehousePresetModel warehousePresetModel);
    }
}
