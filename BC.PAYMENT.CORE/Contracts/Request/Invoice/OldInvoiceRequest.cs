namespace BC.PAYMENT.CORE.Contracts.Request.Invoice;

public record OldInvoiceRequest(
    string Code,
    string CustomerCode,
    string CustomerName,
    double InvoiceValue,
    string Employee
);