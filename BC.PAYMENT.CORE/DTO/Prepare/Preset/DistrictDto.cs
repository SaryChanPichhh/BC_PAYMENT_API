
using System.ComponentModel.DataAnnotations;

namespace BC.PAYMENT.CORE.DTO.Prepare.Preset
{
    public class DistrictDto
    {
        public string? District { get; set; }
        public string? ProvinceId { get; set; }
    }
    public class DistrictUpdateDto : DistrictDto
    {
        [Required]
        public string? DistrictId { get; set; }
    }
}
