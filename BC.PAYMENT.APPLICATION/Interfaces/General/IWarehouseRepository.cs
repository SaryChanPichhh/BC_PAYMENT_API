namespace BC.PAYMENT.APPLICATION.Interfaces.General;

public interface IWarehouseRepository
{
    Task<List<WarehouseDto>> GetWarehouseAsync(string dbCode);
}