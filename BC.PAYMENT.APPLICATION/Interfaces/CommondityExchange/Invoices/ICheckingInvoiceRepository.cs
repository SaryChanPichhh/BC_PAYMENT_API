using BC.PAYMENT.CORE.DTO.CommondityExchange.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.Invoices
{
    public interface ICheckingInvoiceRepository
    {
        Task<List<CheckingInvoiceDto>> LoadNewExchangeInvoiceAsync(string dbCode);
        Task<List<CheckingInvoiceDto>> LoadNewRepairInvoiceAsync(string dbCode);
        Task<List<CheckingInvoiceDto>> GetExchangeNewInvoiceDetailByTransactionCode(string dbCode,string transactionCode);
        Task<List<CheckingInvoiceDto>> GetRepairNewInvoiceDetailByTransactionCode(string dbCode,string transactionCode);

        Task<List<CheckingInvoiceDto>> LoadOldExchangeInvoiceAsync(string dbCode);
        Task<List<CheckingInvoiceDto>> LoadOldRepairInvoiceAsync(string dbCode);
        Task<List<CheckingInvoiceDto>> GetExchangeOldInvoiceDetailByTransactionCode(string dbCode, string transactionCode);
        Task<List<CheckingInvoiceDto>> GetRepairOldInvoiceDetailByTransactionCode(string dbCode, string transactionCode);
    }
}
