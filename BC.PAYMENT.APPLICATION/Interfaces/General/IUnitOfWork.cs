using BC.PAYMENT.APPLICATION.Interfaces.ConfirmBalance;
using BC.PAYMENT.APPLICATION.Interfaces.Expense;
using BC.PAYMENT.APPLICATION.Interfaces.StockCar;
using BC.PAYMENT.APPLICATION.Interfaces.Submit;
using IDividedInvoiceRepository = BC.PAYMENT.APPLICATION.Interfaces.Invoice.IDividedInvoiceRepository;

namespace BC.PAYMENT.APPLICATION.Interfaces.General;

public interface IUnitOfWork
{
    // Generator
    IGeneratorRepository Generators { get; }

    #region General

    IGeneralRepository GeneralRepository { get; }
    IUserRepository Users { get; }
    IBranchRepository Branches { get; }
    ICustomerRepository Customers { get; }
    IInvoiceRepository Invoices { get; }
    IAnalysisAccountRepository AnalysisAccounts { get; }
    Payment.IDividedInvoiceRepository DividedInvoices { get; }
    IDeliveryRepository Deliveries { get; }
    IMarketRepository Markets { get; }
    IAreaRepository Areas { get; }
    IWarehouseRepository Warehouses { get; }
    IItemRepository Items { get; }
    IEmployeeRepository Employee { get; }
    IInvoiceClosingEntryRepository InvoiceClosingEntry { get; }
    IExpenseTypeRepository ExpenseTypes { get; }
    IExpenseRepository Expense { get; }
    IExpenseRepository Expenses => Expense;
    IExpenseRepository ExpenseRepository => Expense;

    #endregion

    #region Prepare

    IDistrictRepository Districts { get; }
    IProvinceRepository Provinces { get; }
    IAccountReceivablePresetRepository AccountReceivablePresets { get; }
    IPublicHolidayRepository PublicHoliday { get; }

    #endregion

    #region Transaction

    INewInvoiceRepository NewInvoice { get; }
    IChangeInvoiceRepository ChangeInvoice { get; }
    IReturnInvoiceRepository ReturnInvoice { get; }
    IDividedInvoiceRepository DividedInvoice { get; }
    ICheckReturnInvoiceRepository CheckReturnInvoice { get; }

    #endregion

    #region Daily Payment

    IDeliveryPaidRepository DeliveryPaid { get; }

    #endregion

    #region Submitting

    IConfirmBalanceRepository ConfirmBalance { get; }
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
    ISubmitExpenseRepository SubmitExpense { get; }

    #endregion

    #region Audit

    IStockInventoryCountingRepository StockInventoryCounting { get; }
    IVerifyInvoiceRepository VerifyInvoice { get; }

    #endregion

    #region Provincial Payment

    IStockCarInvoiceRepository StockCarInvoice { get; }
    ITemplateRepository Template { get; }
    ITransferMoneyRepository TransferMoney { get; }
    IStockCarExpenseRepository StockCarExpense { get; }
    IStockCarPaymentInvoiceRepository StockCarPaymentInvoice { get; }
    ISaleRepresentRepository SaleRepresent { get; }
    IReviewReportRepository ReviewReport { get; }
    ICheckingStockCarPaymentRepository CheckingStockCarPayment { get; }

    #endregion

    #region Inventory

    IVerificationStockRepository VerificationStock { get; }
    IVerificationRFIDRepository VerificationRFID { get; }

    #endregion

    #region Cash Flow Data

    ICashFlowDataRepository CashFlowData { get; }
    ICashFlowDataReportRepository CashFlowDataReport { get; }
    ICashFlowSubmittedRepository CashFlowSubmitted { get; }
    ICashFlowAuditSubmittedRepository CashFlowAuditSubmitted { get; }
    ICashFlowAuditReportRepository CashFlowAuditReport { get; }

    #endregion

    #region Cash Flow

    Interfaces.CashFlow.ICashFlowRepository CashFlow { get; }
    Interfaces.CashFlow.ICashFlowSubmittedRepository CashFlowPaymentSubmitted { get; }
    Interfaces.CashFlow.ICashFlowAuditRepository CashFlowAudit { get; }
    Interfaces.CashFlow.ICashFlowDetailRepository CashFlowDetail { get; }
    Interfaces.CashFlow.ICashFlowHeaderRepository CashFlowHeader { get; }

    #endregion

    #region Commodity Exchange

    IDailyRefundItemRepository DailyRefundItem { get; }
    IRepairGoodsRepository RepairGoods { get; }
    IRepairerRepository Repairer { get; }
    ICompletedRepairRepository CompletedRepair { get; }
    ICheckingInvoiceRepository CheckingInvoice { get; }
    IExchangeItemRepository ExchangeItem { get; }
    IReportDividedInvoiceRepository ReportDividedInvoice { get; }
    IItemExchangeReportRepository ItemExchangeReport { get; }
    IItemRepairReportRepository ItemRepairReport { get; }
    ICreditNoteReportRepository CreditNoteReport { get; }

    #endregion

    #region Preset

    IItemTransactionAnalysisRepository ItemTransactionAnalysis { get; }
    IExchangeItemAnalysisRepository ExchangeItemAnalysis { get; }
    IAnnualPurchaseRepository AnnualPurchase { get; }
    IOwedInvoiceRepository OwedInvoice { get; }
    IInventoryValueRepository InventoryValue { get; }
    IDailyAnalysisRepository DailyAnalysis { get; }

    #endregion

    #region Report

    ICreditInvoiceRepository CreditInvoice { get; }
    IPaidInvoiceRepository PaidInvoice { get; }
    ISummaryInvoiceRepository SummaryInvoice { get; }
    IAmountCollectedRepository AmountCollected { get; }
    IExpenseInvoiceReportRepository ExpenseInvoice { get; }
    ITotalMonthlyPaymentRepository TotalMonthlyPayment { get; }

    #endregion

    #region Other Reports

    IBillsOwedRepository billsOwed { get; }
    ICustomerReportRepository CustomerReport { get; }

    #endregion

    #region Provincial

    ICarPaymentReportRepository CarPaymentReport { get; }
    ICarPaymentRepository CarPayment { get; }

    #endregion

    #region Inventory

    IInventoryRepository Inventory { get; }
    IProductRepository Products { get; }
    IInventoryReportRepository InventoryReport { get; }
    IInventoryExpiredRepository InventoryExpired { get; }
    IInventoryTrackingRepository InventoryTracking { get; }
    IWarehousePresetRepository WarehousePreset { get; }
    IOpeningBalanceRepository OpeningBalance { get; }
    IStockCountingRepository StockCounting { get; }

    #endregion

    #region Invoice

    IOldInvoiceRepository OldInvoice { get; }
    IPaymentInvoiceRepository PaymentInvoice { get; }

    #endregion
}