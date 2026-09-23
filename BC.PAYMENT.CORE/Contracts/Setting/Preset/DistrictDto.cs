namespace BC.PAYMENT.CORE.Contracts.Setting.Preset
{
    public class DistrictDto
    {
        public string? District { get; set; }
        public int? ProvinceId { get; set; }
    }
    public class DistrictUpdateDto : DistrictDto
    {
        [Required]
        public int? DistrictId { get; set; }
    }
}
