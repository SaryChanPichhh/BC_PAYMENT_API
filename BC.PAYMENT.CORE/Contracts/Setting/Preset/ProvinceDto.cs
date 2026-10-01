namespace BC.PAYMENT.CORE.Contracts.Setting.Preset;

public class ProvinceDto
{
    public string? Province { get; set; }
}

public class ProvinceUpdateDto : ProvinceDto
{
    [Required] public string? ProvinceId { get; set; }
}