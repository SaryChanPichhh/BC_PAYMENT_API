
using BC.PAYMENT.APPLICATION.Interfaces.BaseInterface;
using BC.PAYMENT.CORE.Entities.Prepare.Preset;

namespace BC.PAYMENT.APPLICATION.Interfaces.Prepare.Preset
{
    public interface IDistrictRepository : IBaseRepository<DistrictModel>
    {
        Task<List<DistrictModel>> GetDistrictsByProvinceAsync(string province);
        Task<List<DistrictModel>> GetDistrictsByDistrictAsync(string district);
    }
}
