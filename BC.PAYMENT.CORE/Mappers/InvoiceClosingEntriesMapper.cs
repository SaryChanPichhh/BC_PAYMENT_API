using BC.PAYMENT.CORE.Contracts.Request.General;
using BC.PAYMENT.CORE.Entities.General;

namespace BC.PAYMENT.CORE.Mappers;

public static class InvoiceClosingEntriesMapper
{
    public static InvoiceClosingEntriesModel ToInvoiceClosingEntriesModel(this InvoiceClosingEntriesRequest request)
    {
        if (request == null) return null!;

        return new InvoiceClosingEntriesModel
        {
            Description = request.Description,
            IsActive = true, // Defaulting to true, but adjust if necessary
            CreatedAt = DateTime.Now // Usually set by the system
        };
    }
}