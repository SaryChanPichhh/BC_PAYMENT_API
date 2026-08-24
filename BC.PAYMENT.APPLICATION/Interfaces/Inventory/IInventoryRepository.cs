namespace BC.PAYMENT.APPLICATION.Interfaces.Inventory
{
    public interface IInventoryRepository
    {
        Task<List<InventoryModel>> GetInventoryAllBranchesAsync();
        Task<List<dynamic>> GetInventoryByMultiWarehouseAsync(Dictionary<string,List<string>> dbCodeAndWarehouses);


    }
}
