using BC.PAYMENT.CORE.DTO.CommondityExchange.Invoices;
using BC.PAYMENT.CORE.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.Invoices
{
    public interface ICheckingInvoiceRepository
    {
        Task<List<CheckingInvoiceDto>> LoadOldInvoicesAsync(string dbCode, InvoiceType request,int page,int pageSize);
        Task<List<CheckingInvoiceDto>> LoadNewInvoicesAsync(string dbCode, int page, int pageSize, InvoiceType request);
        Task<ExchangeInvoiceDetailRespondDto> GetExchangeNewInvoiceDetailByTransactionCode(string dbCode,string transactionCode);
        Task<RepairInvoiceDetailRespondDto> GetRepairNewInvoiceDetailByTransactionCode(string dbCode,string transactionCode);
        Task<ExchangeInvoiceDetailRespondDto> GetExchangeOldInvoiceDetailByTransactionCode(string dbCode, string transactionCode);
        Task<RepairInvoiceDetailRespondDto> GetRepairOldInvoiceDetailByTransactionCode(string dbCode, string transactionCode);
    }
}
