using BC.PAYMENT.CORE.Contracts.Request.General;

namespace BC.PAYMENT.CORE.Validate;

public class InvoiceClosingEntriesValidate : AbstractValidator<InvoiceClosingEntriesRequest>
{
    public InvoiceClosingEntriesValidate()
    {
        RuleFor(x => x.Description).NotNull().WithMessage("Description is required").NotEmpty()
            .WithMessage("Description Code is required");
    }
}