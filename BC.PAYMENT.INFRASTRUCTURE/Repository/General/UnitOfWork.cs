using BC.PAYMENT.APPLICATION.Interfaces.Accounting;
using BC.PAYMENT.APPLICATION.Interfaces.CashFlow.CashFlowData;
using BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.DailyRefundItems;
using BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.Repairer;
using BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.RepairItem;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.APPLICATION.Interfaces.Generator;
using BC.PAYMENT.APPLICATION.Interfaces.Invoice;
using BC.PAYMENT.APPLICATION.Interfaces.Items;
using BC.PAYMENT.APPLICATION.Interfaces.Login;
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

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IUserRepository users, IBranchRepository branches, ICustomerRepository customers, IInvoiceRepository invoices, IAnalysisAccountRepository analysisAccounts, APPLICATION.Interfaces.Payment.IDividedInvoiceRepository dividedInvoices, IDeliveryRepository deliveries, IMarketRepository markets, IDistrictRepository districts, IProvinceRepository provinces, IAccountReceivablePresetRepository accountReceivablePresets, IPublicHolidayRepository publicHoliday, INewInvoiceRepository newInvoice, IChangeInvoiceRepository changeInvoice, IReturnInvoiceRepository returnInvoice, IIssuanceInvoiceRepository issuanceInvoice, IDividedInvoiceRepository dividedInvoice, IInvoiceReportRepository invoiceReport, ICheckReturnInvoiceRepository checkReturnInvoice, IDeliveryPaidRepository deliveryPaid, IDailyPaymentRepository dailyPayment, IConfirmAccountReceivableRepository confirmAccountReceivable, ISubmittingInvoiceRepository submittingInvoice, IGeneratorRepository generators, IAccountReceivableRepository accountReceivable, ISubmittingPerDeliveryRepository submittingPerDelivery, ISubmittedInvoiceRepository submittedInvoice, ISubmittedPerDeliveryRepository submittedPerDelivery, ISubmittedRejectedInvoiceRepository submittedRejectedInvoice, ISubmittedRejectedInvoicePerDeliveryRepository rejectedInvoicePerDelivery, IMonthlyInvoiceRepository invoiceVerify, ISubmissionHistoryRepository historyApproval, IDailySubmissionRepository approve, IStockInventoryCountingRepository stockInventoryCounting, IVerifyInvoiceRepository verifyInvoice, ISaleRepresentRepository saleRepresent, IReviewReportRepository reviewReport, IVerificationStockRepository verificationStock, ICheckingStockCarPaymentRepository checkingStockCarPayment, IVerificationRFIDRepository verificationRfid, IWarehouseRepository warehouses, IItemRepository items, ICashFlowDataRepository cashFlowData, ICashFlowDataReportRepository cashFlowDataReport, ICashFlowSubmittedRepository cashFlowSubmitted, ICashFlowAuditSubmittedRepository cashFlowAuditSubmitted, ICashFlowAuditReportRepository cashFlowAuditReport, IDailyRefundItemRepository dailyRefundItem, IRepairGoodsRepository repairGoods, IRepairerRepository repairer, ICompletedRepairRepository completedRepair)
        {
            Users = users;
            Branches = branches;
            Customers = customers;
            Invoices = invoices;
            AnalysisAccounts = analysisAccounts;
            DividedInvoices = dividedInvoices;
            Deliveries = deliveries;
            Markets = markets;
            Districts = districts;
            Provinces = provinces;
            AccountReceivablePresets = accountReceivablePresets;
            PublicHoliday = publicHoliday;
            NewInvoice = newInvoice;
            ChangeInvoice = changeInvoice;
            ReturnInvoice = returnInvoice;
            IssuanceInvoice = issuanceInvoice;
            DividedInvoice = dividedInvoice;
            InvoiceReport = invoiceReport;
            CheckReturnInvoice = checkReturnInvoice;
            DeliveryPaid = deliveryPaid;
            DailyPayment = dailyPayment;
            ConfirmAccountReceivable = confirmAccountReceivable;
            SubmittingInvoice = submittingInvoice;
            Generators = generators;
            AccountReceivable = accountReceivable;
            SubmittingPerDelivery = submittingPerDelivery;
            SubmittedInvoice = submittedInvoice;
            SubmittedPerDelivery = submittedPerDelivery;
            RejectedInvoice = submittedRejectedInvoice;
            RejectedInvoicePerDelivery = rejectedInvoicePerDelivery;
            InvoiceVerify = invoiceVerify;
            HistoryApproval = historyApproval;
            Approve = approve;
            StockInventoryCounting = stockInventoryCounting;
            VerifyInvoice = verifyInvoice;
            SaleRepresent = saleRepresent;
            ReviewReport = reviewReport;
            VerificationStock = verificationStock;
            CheckingStockCarPayment = checkingStockCarPayment;
            VerificationRFID = verificationRfid;
            Warehouses = warehouses;
            Items = items;
            CashFlowData = cashFlowData;
            CashFlowDataReport = cashFlowDataReport;
            CashFlowSubmitted = cashFlowSubmitted;
            CashFlowAuditSubmitted = cashFlowAuditSubmitted;
            CashFlowAuditReport = cashFlowAuditReport;
            DailyRefundItem = dailyRefundItem;
            RepairGoods = repairGoods;
            Repairer = repairer;
            CompletedRepair = completedRepair;
        }


        public IGeneratorRepository Generators { get; }
        public IUserRepository Users { get; set; }
        public IBranchRepository Branches { get; set; }
        public ICustomerRepository Customers { get; set; }
        public IInvoiceRepository Invoices { get; set; }
        public IAnalysisAccountRepository AnalysisAccounts { get; set; }
        public APPLICATION.Interfaces.Payment.IDividedInvoiceRepository DividedInvoices { get; }
        public IDeliveryRepository Deliveries { get; }
        public IMarketRepository Markets { get; }
        public IWarehouseRepository Warehouses { get; }
        public IItemRepository Items { get; }
        public IDistrictRepository Districts { get; set; }
        public IProvinceRepository Provinces { get; }
        public IAccountReceivablePresetRepository AccountReceivablePresets { get; }
        public IPublicHolidayRepository PublicHoliday { get; }
        public INewInvoiceRepository NewInvoice { get; }
        public IChangeInvoiceRepository ChangeInvoice { get; }
        public IReturnInvoiceRepository ReturnInvoice { get; }
        public IIssuanceInvoiceRepository IssuanceInvoice { get; }
        public IDividedInvoiceRepository DividedInvoice { get; }
        public IInvoiceReportRepository InvoiceReport { get; }
        public ICheckReturnInvoiceRepository CheckReturnInvoice { get; }
        public IDeliveryPaidRepository DeliveryPaid { get; }
        public IDailyPaymentRepository DailyPayment { get; }
        public IConfirmAccountReceivableRepository ConfirmAccountReceivable { get; }
        public ISubmittingInvoiceRepository SubmittingInvoice { get; }
        public IAccountReceivableRepository AccountReceivable { get; }
        public ISubmittingPerDeliveryRepository SubmittingPerDelivery { get; }
        public ISubmittedInvoiceRepository SubmittedInvoice { get; }
        public ISubmittedPerDeliveryRepository SubmittedPerDelivery { get; }
        public ISubmittedRejectedInvoiceRepository RejectedInvoice { get; }
        public ISubmittedRejectedInvoicePerDeliveryRepository RejectedInvoicePerDelivery { get; }
        public IMonthlyInvoiceRepository InvoiceVerify { get; }
        public ISubmissionHistoryRepository HistoryApproval { get; }
        public IDailySubmissionRepository Approve { get; set; }
        public IStockInventoryCountingRepository StockInventoryCounting { get; }
        public IVerifyInvoiceRepository VerifyInvoice { get; }
        public ISaleRepresentRepository SaleRepresent { get; }
        public IReviewReportRepository ReviewReport { get; }
        public ICheckingStockCarPaymentRepository CheckingStockCarPayment { get; }
        public IVerificationStockRepository VerificationStock { get; }
        public IVerificationRFIDRepository VerificationRFID { get; }
        public ICashFlowDataRepository CashFlowData { get; }
        public ICashFlowDataReportRepository CashFlowDataReport { get; }
        public ICashFlowSubmittedRepository CashFlowSubmitted { get; }
        public ICashFlowAuditSubmittedRepository CashFlowAuditSubmitted { get; }
        public ICashFlowAuditReportRepository CashFlowAuditReport { get; }
        public IDailyRefundItemRepository DailyRefundItem { get; }
        public IRepairGoodsRepository RepairGoods { get; }
        public IRepairerRepository Repairer { get; }
        public ICompletedRepairRepository CompletedRepair { get; }
    }
}
