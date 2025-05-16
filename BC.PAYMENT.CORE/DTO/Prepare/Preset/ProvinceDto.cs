

using System.ComponentModel.DataAnnotations;

namespace BC.PAYMENT.CORE.DTO.Prepare.Preset
{
    public  class ProvinceDto
    {
        public string? Province { get; set; }
    }
    public class ProvinceUpdateDto : ProvinceDto
    {
        [Required]
        public string? ProvinceId { get; set; }
    }
}
