namespace BC.PAYMENT.APPLICATION.Interfaces.General
{
    public interface IProductRepository
    {
        Task<List<ProductModel>> GetAllProductsAsync(string dbCode);
    }
}
