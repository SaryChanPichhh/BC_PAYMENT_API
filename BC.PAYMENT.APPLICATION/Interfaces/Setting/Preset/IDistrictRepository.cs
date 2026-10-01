namespace BC.PAYMENT.APPLICATION.Interfaces.Setting.Preset;

public interface IDistrictRepository
{
    Task<int> AddNewAsync(DistrictModel model);
    Task<int> UpdateAsync(DistrictModel model);
    Task<List<DistrictResponseDTO>> GetAsync(string dbCode);
    Task<int> DeleteAsync(string code);
    Task<List<DistrictResponseDTO>> GetDistrictsByProvinceAsync(string province);
    Task<List<DistrictResponseDTO>> GetDistrictsByDistrictAsync(string district);
}