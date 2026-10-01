namespace BC.PAYMENT.APPLICATION.Interfaces.Inventory;

public interface IInventoryExpiredRepository
{
    Task<List<InventoryExpiredModel>> GetItemExpiredAsync(string dbCode, string location);
    Task<List<InventoryExpiredModel>> GetItemExpiredSoonAsync(string dbCode, string location);
}