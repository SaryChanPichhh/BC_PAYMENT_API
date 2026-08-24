namespace BC.PAYMENT.CORE.Validate;

public class AccReceivablePresetValidate : AbstractValidator<AccountReceivableCreateRequest>
{
    public AccReceivablePresetValidate()
    {
        RuleFor(x=>x.AccountCode).NotEmpty().WithMessage("Account code is required");
        RuleFor(x=>x.Description).NotEmpty().WithMessage("Description is required");
    }
}


