using BC.PAYMENT.APPLICATION.Interfaces.Accounting;
using BC.PAYMENT.APPLICATION.Interfaces.Generator;
using BC.PAYMENT.APPLICATION.Interfaces.Invoice;
using BC.PAYMENT.APPLICATION.Interfaces.Items;
using BC.PAYMENT.APPLICATION.Interfaces.Login;
using BC.PAYMENT.APPLICATION.Interfaces.Payment;
using BC.PAYMENT.APPLICATION.Interfaces.Prepare.Account;
using BC.PAYMENT.APPLICATION.Interfaces.Prepare.EmployeeSchedule;
using BC.PAYMENT.APPLICATION.Interfaces.Prepare.Preset;
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

    }
}
