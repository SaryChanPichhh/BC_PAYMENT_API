using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using FluentValidation;

namespace BC.PAYMENT.CORE.Validate;

public class PaymentInvoiceValidate : AbstractValidator<PaymentInvoiceRequest>
{
    public PaymentInvoiceValidate()
    {
        RuleFor(x => x.DividedInvoiceId)
            .GreaterThan(0).WithMessage("Divided Invoice Id must be greater than 0.");

        RuleFor(x => x.PaymentStatus)
            .NotEmpty().WithMessage("Payment Status is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");
    }
}