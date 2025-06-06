using BC.PAYMENT.APPLICATION.Interfaces.Accounting;
using BC.PAYMENT.APPLICATION.Interfaces.CashFlow.CashFlowData;
using BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.DailyRefundItems;
using BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.ExchangeItem;
using BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.Invoices;
using BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.Repairer;
using BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.RepairItem;
using BC.PAYMENT.APPLICATION.Interfaces.Generator;
using BC.PAYMENT.APPLICATION.Interfaces.Invoice;
using BC.PAYMENT.APPLICATION.Interfaces.Items;
using BC.PAYMENT.APPLICATION.Interfaces.Login;
using BC.PAYMENT.APPLICATION.Interfaces.Prepare.Account;
using BC.PAYMENT.APPLICATION.Interfaces.Prepare.EmployeeSchedule;
using BC.PAYMENT.APPLICATION.Interfaces.Prepare.Preset;
using BC.PAYMENT.APPLICATION.Interfaces.Preset.AnnualPurchase;
using BC.PAYMENT.APPLICATION.Interfaces.Preset.DailyAnalysis;
using BC.PAYMENT.APPLICATION.Interfaces.Preset.ExchangeItemAnalysis;
using BC.PAYMENT.APPLICATION.Interfaces.Preset.ItemTransaction;
using BC.PAYMENT.APPLICATION.Interfaces.Preset.OwedInvoice;
using BC.PAYMENT.APPLICATION.Interfaces.Preset.StockPrice;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.Audit.StockInventoryCounting;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.Audit.VerifyInvoice;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.AccountReceivable;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DailyPayment;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DeliveryPaid;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.DailyReport;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.Inventory.VerificationRFID;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.Inventory.VerificationStock;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.ProvincialPayment.CheckingStockCarApproval;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.ProvincialPayment.ReviewReport;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.ProvincialPayment.StockCarPayment;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.Submitting.DailySubmission;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.Submitting.HistoryApproval;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.Submitting.InvoiceVerify;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.Submitting.SubmittingInvoice;
using IDividedInvoiceRepository = BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.Invoice.IDividedInvoiceRepository;

namespace BC.PAYMENT.APPLICATION.Interfaces.General
{
    public interface IUnitOfWork
    {
        // Generator
        IGeneratorRepository Generators { get; }

        IUserRepository Users { get; }
        IBranchRepository Branches { get; }
        ICustomerRepository Customers { get; }
        IInvoiceRepository Invoices { get; }
        IAnalysisAccountRepository AnalysisAccounts { get; }
        Payment.IDividedInvoiceRepository DividedInvoices { get; }
        IDeliveryRepository Deliveries { get; }
        IMarketRepository Markets { get; }
        IWarehouseRepository Warehouses { get; }
        IItemRepository Items { get; }

        // Prepare
        IDistrictRepository Districts { get; }
        IProvinceRepository Provinces { get; }
        IAccountReceivablePresetRepository AccountReceivablePresets { get; }
        IPublicHolidayRepository PublicHoliday { get; }
        // Transaction
        INewInvoiceRepository NewInvoice { get; }
        IChangeInvoiceRepository ChangeInvoice { get; }
        IReturnInvoiceRepository ReturnInvoice { get; }
        IIssuanceInvoiceRepository IssuanceInvoice { get; }
        IDividedInvoiceRepository DividedInvoice { get; }
        IInvoiceReportRepository InvoiceReport { get; }
        ICheckReturnInvoiceRepository CheckReturnInvoice { get; }
        
        // Daily Payment
        IDeliveryPaidRepository DeliveryPaid { get; }
        IDailyPaymentRepository DailyPayment { get; }

        //Submitting
        IConfirmAccountReceivableRepository ConfirmAccountReceivable { get; }
        ISubmittingInvoiceRepository SubmittingInvoice { get; }
        IAccountReceivableRepository AccountReceivable { get; }
        ISubmittingPerDeliveryRepository SubmittingPerDelivery { get; }
        ISubmittedInvoiceRepository SubmittedInvoice { get; }
        ISubmittedPerDeliveryRepository SubmittedPerDelivery { get; }
        ISubmittedRejectedInvoiceRepository RejectedInvoice { get; }
        ISubmittedRejectedInvoicePerDeliveryRepository RejectedInvoicePerDelivery { get; }
        IMonthlyInvoiceRepository InvoiceVerify { get; }
        ISubmissionHistoryRepository HistoryApproval { get; }
        IDailySubmissionRepository Approve { get; set; }
        // Audit
        IStockInventoryCountingRepository StockInventoryCounting { get; }
        IVerifyInvoiceRepository VerifyInvoice { get; }
        // Provincial Payment
        ISaleRepresentRepository SaleRepresent { get; }
        IReviewReportRepository ReviewReport { get; }
        ICheckingStockCarPaymentRepository CheckingStockCarPayment { get; }

        // Inventory
        IVerificationStockRepository VerificationStock { get; }
        IVerificationRFIDRepository VerificationRFID { get; }

        // Cash Flow Data
        ICashFlowDataRepository CashFlowData { get; }
        ICashFlowDataReportRepository CashFlowDataReport { get; }
        ICashFlowSubmittedRepository CashFlowSubmitted { get; }
        ICashFlowAuditSubmittedRepository CashFlowAuditSubmitted { get; }
        ICashFlowAuditReportRepository CashFlowAuditReport { get; }

        // Commodity Exchange

        IDailyRefundItemRepository DailyRefundItem { get; }
        IRepairGoodsRepository RepairGoods { get; }
        IRepairerRepository Repairer { get; }
        ICompletedRepairRepository CompletedRepair { get; }
        ICheckingInvoiceRepository CheckingInvoice { get; }
        IExchangeItemRepository ExchangeItem { get; }

        // Preset 
        IItemTransactionAnalysisRepository ItemTransactionAnalysis { get; }
        IExchangeItemAnalysisRepository ExchangeItemAnalysis { get; }
        IAnnualPurchaseRepository AnnualPurchase { get; }
        IOwedInvoiceRepository OwedInvoice { get; }
        IInventoryValueRepository InventoryValue { get; }
        IDailyAnalysisRepository DailyAnalysis { get; }
    }
}
