using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using FluentValidation;

namespace BC.PAYMENT.CORE.Validate;

public class ChangeInvoiceValidate : AbstractValidator<ChangeInvoiceRequest>
{
    public ChangeInvoiceValidate()
    {
        RuleFor(x => x.TransactionCode).NotEmpty().WithMessage("Transaction Code is required");
        RuleFor(x => x.CustomerCode).NotEmpty().WithMessage("Customer Code is required");
        RuleFor(x => x.CustomerName).NotEmpty().WithMessage("Customer Name is required");
        RuleFor(x => x.InvoiceValue).GreaterThan(0).WithMessage("Invoice Value must be greater than 0");
    }
}