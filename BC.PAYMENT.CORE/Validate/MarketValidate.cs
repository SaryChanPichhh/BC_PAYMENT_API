namespace BC.PAYMENT.CORE.Validate;

public class MarketValidate : AbstractValidator<MarketCreateRequest>
{
    public MarketValidate()
    {
        RuleFor(m => m.MarketId)
            .NotEmpty().WithMessage("MarketId is required.")
            .MaximumLength(50).WithMessage("MarketId cannot exceed 50 characters.");

        RuleFor(m => m.MarketName)
            .NotEmpty().WithMessage("Market Name is required.")
            .MaximumLength(200).WithMessage("Market Name cannot exceed 200 characters.");

        RuleFor(m => m.MarketNameKhmer)
            .NotEmpty().WithMessage("Market Name in Khmer is required.")
            .MaximumLength(200).WithMessage("Market Name in Khmer cannot exceed 200 characters.");

        RuleFor(m => m.AreaId)
            .NotEmpty().WithMessage("Market Name is required");

        RuleFor(m => m.DistrictId)
            .GreaterThan(0).WithMessage("DistrictId must be valid and greater than 0.");

        RuleFor(m => m.ProvinceId)
            .GreaterThan(0).WithMessage("ProvinceId must be valid and greater than 0.");
    }
}

public class MarketUpdateValidate : AbstractValidator<MarketUpdateRequest>
{
    public MarketUpdateValidate()
    {
        Include(new MarketValidate());
        
        RuleFor(m => m.Status)
            .NotNull().WithMessage("Status is required.");
    }
}