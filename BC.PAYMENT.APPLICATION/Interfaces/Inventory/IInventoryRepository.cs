using BC.PAYMENT.CORE.Contracts.Response.Inventory;
using BC.PAYMENT.CORE.Entities.Inventory.Inventory;

namespace BC.PAYMENT.APPLICATION.Interfaces.Inventory
{
    public interface IInventoryRepository
    {
        Task<List<dynamic>> GetInventoryByMultiWarehouseAsync(Dictionary<string,List<string>> dbCodeAndWarehouses);
        Task<List<InventoryResponse>> GetInventoryByLocationAsync(string dbCode, string location);
    }
}
