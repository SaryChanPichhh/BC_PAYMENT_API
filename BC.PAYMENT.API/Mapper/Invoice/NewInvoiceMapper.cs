using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using BC.PAYMENT.CORE.Entities.Invoice;

namespace BC.PAYMENT.API.Mapper.Invoice;

public static class NewInvoiceMapper
{
    public static NewInvoiceModel FromResponseToModel(this NewInvoiceResponse res)
    {
        return new NewInvoiceModel
        {
            CustomerCode = res.CustomerCode,
            CustomerName = res.CustomerName,
            InvoiceCode = res.InvoiceCode,
            InvoiceType = res.InvoiceType,
            InvoiceAmount = res.InvoiceAmount,
            EntriesCode = res.EntriesCode,
            CreatedBy = res.CreatedBy,
            CreatedDate = res.CreatedDate,
            DbCode = res.DbCode
        };
    }

    public static NewInvoiceModel FromRequestToModel(this NewInvoiceRequest req)
    {
        return new NewInvoiceModel
        {
            CustomerCode = req.CustomerCode,
            CustomerName = req.CustomerName,
            InvoiceCode = req.InvoiceCode,
            InvoiceType = req.InvoiceType,
            InvoiceAmount = req.InvoiceAmount,
            CreatedDate = req.CreatedAt,
            CreatedBy = req.CreatedBy,
            CustomerField = req.CustomerField1,
            AccNameKh = req.AccNameKh
        };
    }
}