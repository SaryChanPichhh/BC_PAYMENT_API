namespace BC.PAYMENT.APPLICATION.Interfaces.Setting.Preset
{
    public interface IDistrictRepository : IBaseRepository<DistrictModel>
    {
        Task<List<DistrictModel>> GetDistrictsByProvinceAsync(string province);
        Task<List<DistrictModel>> GetDistrictsByDistrictAsync(string district);
    }
}
