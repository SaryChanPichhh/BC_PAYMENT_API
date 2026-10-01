using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using FluentValidation;

namespace BC.PAYMENT.CORE.Validate;

public class CreateDividedInvoiceValidate : AbstractValidator<CreateDividedInvoiceRequest>
{
    public CreateDividedInvoiceValidate()
    {
        RuleFor(x => x.DeliveryId)
            .NotEmpty().WithMessage("Delivery Id is required.");

        RuleFor(x => x.InvoiceId)
            .GreaterThan(0).WithMessage("Invoice Id must be greater than 0.");
    }
}

public class DividedInvoiceValidate : CreateDividedInvoiceValidate
{
}

public class DeleteDividedInvoiceValidate : AbstractValidator<DeleteDividedInvoiceRequest>
{
    public DeleteDividedInvoiceValidate()
    {
        RuleFor(x => x.InvoiceId)
            .NotEmpty().WithMessage("Invoice Id is required.");

        RuleFor(x => x.TransactionCode)
            .NotEmpty().WithMessage("Transaction Code is required.");

        RuleFor(x => x.DeliveryId)
            .NotEmpty().WithMessage("Delivery Id is required.");

        RuleFor(x => x.Note)
            .NotEmpty().WithMessage("Note is required.");
    }
}

public class DividedInvoiceDeleteValidate : DeleteDividedInvoiceValidate
{
}