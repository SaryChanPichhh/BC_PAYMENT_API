using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BC.PAYMENT.CORE.DTO;
using BC.PAYMENT.CORE.DTO.Filter;
using BC.PAYMENT.CORE.DTO.Invoice;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Entities.Invoice;

namespace BC.PAYMENT.APPLICATION.Interfaces.Payment
{
    public interface IDividedInvoiceRepository
    {
        Task<List<Invoices>> GetInvoices(IssueInvoiceExclusionFilterDTO dto);
        Task<List<Invoices>> GetInvoices(IssueInvoiceFilterDTO dto);
        Task<int> SaveDividedInvoice(List<IssueInvoiceDTO> dto);

        Task<List<Delivery>> GetDividedDeliveryInfo(string dbCode, DateTime date);
        Task<List<Invoices>> GetDividedInvoice(string dbCode, string deliveryId, DateTime date);
        Task<List<DividedInvoiceSummary>> GetDividedInvoiceSummary(string dbCode, DateTime date);
    }
}
