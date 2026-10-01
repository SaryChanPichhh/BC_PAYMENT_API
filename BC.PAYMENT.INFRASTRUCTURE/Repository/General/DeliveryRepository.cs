using BC.PAYMENT.CORE.Contracts.Response.Delivery;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General;

public class DeliveryRepository(ISqlDataAccess sqlDataAccess) : IDeliveryRepository
{
    public async Task<List<Delivery>> GetDelivery(string dbCode)
    {
        var param = new
        {
            DB_CODE = dbCode
        };
        var result =
            await sqlDataAccess.LoadData<Delivery, dynamic>(DeliveryQueries.GetDelivery, param,
                CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<List<DeliveryResponse>> GetDeliveryByPermissionAsync(string dbCode)
    {
        var param = new { DB_CODE = dbCode };
        var result =
            await sqlDataAccess.LoadData<DeliveryResponse, dynamic>(DeliveryQueries.GetDeliveryByPermission, param);
        return result.ToList();
    }

    public async Task<Delivery.DeliveryImage> GetDeliveryImage(string deliveryId)
    {
        var param = new
        {
            DELIVERIES_ID = deliveryId
        };
        var result =
            await sqlDataAccess.LoadSingleData<Delivery.DeliveryImage, dynamic>(DeliveryQueries.GetDeliveryImage,
                param);
        return result;
    }

    public async Task<bool> CreateDelivery(Delivery delivery)
    {
        var result = await sqlDataAccess.ExecuteAsync(DeliveryQueries.CreateDelivery, delivery);
        return result > 0;
    }

    public async Task<bool> UpdateDelivery(Delivery delivery)
    {
        var result = await sqlDataAccess.ExecuteAsync(DeliveryQueries.UpdateDelivery, delivery);
        return result > 0;
    }

    public async Task<bool> DeleteDelivery(string deliveryId)
    {
        var param = new { DeliveryId = deliveryId };
        var result = await sqlDataAccess.ExecuteAsync(DeliveryQueries.DeleteDelivery, param);
        return result > 0;
    }
}