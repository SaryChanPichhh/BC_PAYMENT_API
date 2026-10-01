namespace BC.PAYMENT.CORE.Contracts.TablesType;

public record OldInvoiceTableType(
    string DbCode,
    string Code,
    int Period,
    string CustomerCode,
    string CustomerName,
    double InvoiceValue,
    string Employee,
    string CreatedBy);