using BC.PAYMENT.CORE.Contracts.Response.Province;

namespace BC.PAYMENT.APPLICATION.Interfaces.Setting.Preset
{
    public  interface IProvinceRepository : IBaseRepository<ProvinceModel>
    {
        Task<List<ProvinceResponse>> GetAllProvinces();
    }
}