namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public DeliveryRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<Delivery>> GetDelivery(string dbCode)
        {
            var sql = "PM_SELECT_DELIVERIES";

            var param = new
            {
                DB_CODE = dbCode,
            };
            var result = await _sqlDataAccess.LoadData<Delivery, dynamic>(sql, param, CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<Delivery.DeliveryImage> GetDeliveryImage(string deliveryId)
        {
            var sql = "SELECT IMAGE FROM TB_BCDELIVERIES WHERE DELIVERIES_ID = @DELIVERIES_ID";
            var param = new
            {
                DELIVERIES_ID = deliveryId,
            };
            var result = await _sqlDataAccess.LoadSingleData<Delivery.DeliveryImage, dynamic>(sql, param);
            return result;

        }
    }
}
