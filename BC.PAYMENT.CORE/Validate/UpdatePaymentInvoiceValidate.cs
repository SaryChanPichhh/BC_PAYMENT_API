using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using FluentValidation;

namespace BC.PAYMENT.CORE.Validate;

public class UpdatePaymentInvoiceValidate : AbstractValidator<UpdatePaymentInvoiceRequest>
{
    public UpdatePaymentInvoiceValidate()
    {
        RuleFor(x => x.DividedId)
            .GreaterThan(0).WithMessage("Divided Id must be greater than 0.");

        RuleFor(x => x.InvoiceId)
            .GreaterThan(0).WithMessage("Invoice Id must be greater than 0.");

        RuleFor(x => x.PaymentId)
            .GreaterThan(0).WithMessage("Payment Id must be greater than 0.");

        RuleFor(x => x.NewAmount)
            .GreaterThanOrEqualTo(0).WithMessage("New Amount must be greater than or equal to 0.");

        RuleFor(x => x.OldAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Old Amount must be greater than or equal to 0.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");
    }
}
