using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using FluentValidation;

namespace BC.PAYMENT.CORE.Validate;

public class OldInvoiceValidate : AbstractValidator<OldInvoiceRequest>
{
    public OldInvoiceValidate()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage("Code is required");
        RuleFor(x => x.CustomerCode).NotEmpty().WithMessage("Customer Code is required");
        RuleFor(x => x.CustomerName).NotEmpty().WithMessage("Customer Name is required");
        RuleFor(x => x.InvoiceValue).GreaterThan(0).WithMessage("Invoice Value must be greater than 0");
        RuleFor(x => x.Employee).NotEmpty().WithMessage("Employee is required");
    }
}