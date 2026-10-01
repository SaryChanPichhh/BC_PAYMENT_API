using BC.PAYMENT.APPLICATION.Interfaces.CashFlow;
using BC.PAYMENT.APPLICATION.Interfaces.ConfirmBalance;
using BC.PAYMENT.APPLICATION.Interfaces.Expense;
using BC.PAYMENT.APPLICATION.Interfaces.StockCar;
using BC.PAYMENT.APPLICATION.Interfaces.Submit;
using BC.PAYMENT.APPLICATION.Interfaces.ViewStock;
using BC.PAYMENT.INFRASTRUCTURE.Repository.CashFlow;
using BC.PAYMENT.INFRASTRUCTURE.Repository.ConfirmBalance;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Expense;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Item;
using BC.PAYMENT.INFRASTRUCTURE.Repository.StockCar;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Submit;
using BC.PAYMENT.INFRASTRUCTURE.Repository.ViewStock;
using CashFlowSubmittedRepository = BC.PAYMENT.INFRASTRUCTURE.Repository.CashFlow.CashFlowData.CashFlowSubmittedRepository;
using DividedInvoiceRepository = BC.PAYMENT.INFRASTRUCTURE.Repository.Invoice.DividedInvoiceRepository;
using ICashFlowSubmittedRepository = BC.PAYMENT.APPLICATION.Interfaces.CashFlow.CashFlowData.ICashFlowSubmittedRepository;

namespace BC.PAYMENT.INFRASTRUCTURE;

public static class ServiceCollectionExtension
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IBranchRepository, BranchRepository>();
        services.AddTransient<IInvoiceClosingEntryRepository, InvoiceClosingEntryRepository>();
        services.AddTransient<IInvoiceRepository, InvoiceRepository>();
        services.AddTransient<ICustomerRepository, CustomerRepository>();
        services.AddTransient<IAnalysisAccountRepository, AnalysisAccountRepository>();
        services
            .AddTransient<APPLICATION.Interfaces.Payment.IDividedInvoiceRepository,
                Repository.Payment.DividedInvoiceRepository>();
        services.AddTransient<IPaymentRepository, PaymentRepository>();
        services.AddTransient<IDeliveryRepository, DeliveryRepository>();
        services.AddTransient<IAreaRepository, AreaRepository>();
        services.AddTransient<IMarketRepository, MarketRepository>();
        services.AddTransient<IEmployeeRepository, EmployeeRepository>();
        services.AddTransient<IExpenseTypeRepository, ExpenseTypeRepository>();
        services.AddTransient<IExpenseRepository, ExpenseRepository>();
        // Prepare 
        // Preset
        services.AddTransient<IDistrictRepository, DistrictRepository>();
        services.AddTransient<IProvinceRepository, ProvinceRepository>();
        services.AddTransient<IAccountReceivablePresetRepository, AccountReceivablePresetRepository>();
        services.AddTransient<IPublicHolidayRepository, PublicHolidayRepository>();
        // Preset
        // Prepare
        // Transaction
        // Dividing Invoices
        services.AddTransient<INewInvoiceRepository, NewInvoiceRepository>();
        services.AddTransient<IChangeInvoiceRepository, ChangeInvoiceRepository>();
        services.AddTransient<APPLICATION.Interfaces.Invoice.IDividedInvoiceRepository, DividedInvoiceRepository>();
        services.AddTransient<IOldInvoiceRepository, OldInvoiceRepository>();
        services.AddTransient<IPaymentInvoiceRepository, PaymentInvoiceRepository>();
        services.AddTransient<IReturnInvoiceRepository, ReturnInvoiceRepository>();
        services.AddTransient<ICheckReturnInvoiceRepository, CheckReturnInvoiceRepository>();
        // Delivery Paid
        services.AddTransient<IDeliveryPaidRepository, DeliveryPaidRepository>();
        // Account Receivable
        services.AddTransient<IConfirmBalanceRepository, ConfirmBalanceRepository>();
        services.AddTransient<IConfirmAccountReceivableRepository, ConfirmAccountReceivableRepository>();
        services.AddTransient<ISubmittingInvoiceRepository, SubmittingInvoiceRepository>();
        services.AddTransient<IAccountReceivableRepository, AccountReceivableRepository>();
        services.AddTransient<ISubmittingPerDeliveryRepository, SubmittingPerDeliveryRepository>();
        services.AddTransient<ISubmittedInvoiceRepository, SubmittedInvoiceRepository>();
        services.AddTransient<ISubmittedPerDeliveryRepository, SubmittedPerDeliveryRepository>();
        services.AddTransient<ISubmittedRejectedInvoiceRepository, SubmittedRejectedRepository>();
        services.AddTransient<ISubmittedRejectedInvoicePerDeliveryRepository, SubmittedRejectedPerDeliveryRepository>();
        services.AddTransient<IMonthlyInvoiceRepository, MonthlyInvoiceRepository>();
        services.AddTransient<ISubmissionHistoryRepository, SubmissionHistoryRepository>();
        services.AddTransient<IDailySubmissionRepository, DailySubmissionRepository>();
        services.AddTransient<ISubmitExpenseRepository, SubmitExpenseRepository>();
        // Audit
        // Stock Inventory Counting
        services.AddTransient<IStockInventoryCountingRepository, StockInventoryCountingRepository>();
        // Verify Invoice
        services.AddTransient<IVerifyInvoiceRepository, VerifyInvoiceRepository>();

        services.AddTransient<ISaleRepresentRepository, SaleRepresentRepository>();
        // Provincial Payment
        // Review Report
        services.AddTransient<IReviewReportRepository, ReviewReportRepository>();
        services.AddTransient<IVerificationStockRepository, VerificationStockRepository>();
        services.AddTransient<IVerificationRFIDRepository, VerificationRFIDRepository>();
        services.AddTransient<ICheckingStockCarPaymentRepository, CheckingStockCarPaymentRepository>();
        // Cash Flow
        services.AddTransient<ICashFlowDataRepository, CashFlowDataRepository>();
        services.AddTransient<ICashFlowDataReportRepository, CashFlowDataReportRepository>();
        services.AddTransient<ICashFlowSubmittedRepository, CashFlowSubmittedRepository>();
        services.AddTransient<ICashFlowAuditSubmittedRepository, CashFlowAuditSubmittedRepository>();
        services.AddTransient<ICashFlowAuditReportRepository, CashFlowAuditReportRepository>();

        services.AddTransient<APPLICATION.Interfaces.CashFlow.ICashFlowSubmittedRepository,
            Repository.CashFlow.CashFlowSubmittedRepository>();
        services.AddTransient<ICashFlowRepository, CashFlowRepository>();
        services.AddTransient<ICashFlowAuditRepository, CashFlowAuditRepository>();
        services.AddTransient<ICashFlowDetailRepository, CashFlowDetailRepository>();
        services.AddTransient<ICashFlowHeaderRepository, CashFlowHeaderRepository>();

        // Commodity Exchange
        services.AddTransient<IDailyRefundItemRepository, DailyRefundItemsRepository>();
        services.AddTransient<IRepairGoodsRepository, RepairGoodsRepository>();
        services.AddTransient<IRepairerRepository, RepairerRepository>();
        services.AddTransient<ICompletedRepairRepository, RepairerRepository>();
        services.AddTransient<ICheckingInvoiceRepository, CheckingInvoiceRepository>();
        services.AddTransient<IExchangeItemRepository, ExchangeItemRepository>();
        services.AddTransient<IReportDividedInvoiceRepository, ReportDividedInvoiceRepository>();
        services.AddTransient<IItemExchangeReportRepository, ItemExchangeReportRepository>();
        services.AddTransient<IItemRepairReportRepository, ItemRepairReportRepository>();
        services.AddTransient<ICreditNoteReportRepository, CreditNoteReportRepository>();

        // Preset
        services.AddTransient<IItemTransactionAnalysisRepository, ItemTransactionAnalysisRepository>();
        services.AddTransient<IExchangeItemAnalysisRepository, ExchangeItemAnalysisRepository>();
        services.AddTransient<IAnnualPurchaseRepository, AnnualPurchaseRepository>();
        services.AddTransient<IOwedInvoiceRepository, OwedInvoiceRepository>();
        services.AddTransient<IInventoryValueRepository, InventoryValueRepository>();
        services.AddTransient<IDailyAnalysisRepository, DailyAnalysisRepository>();
        // Report
        // Daily Payment Report
        services.AddTransient<ICreditInvoiceRepository, CreditInvoiceRepository>();
        services.AddTransient<IPaidInvoiceRepository, PaidInvoiceRepository>();
        services.AddTransient<ISummaryInvoiceRepository, SummaryInvoiceReportRepository>();
        services.AddTransient<IAmountCollectedRepository, AmountCollectedRepository>();
        services.AddTransient<ITotalMonthlyPaymentRepository, MonthlyPaymentRepository>();
        services.AddTransient<IExpenseInvoiceReportRepository, ExpenseInvoiceReportRepository>();
        // Other Reports 
        services.AddTransient<IBillsOwedRepository, BillsOwedRepository>();
        services.AddTransient<ICustomerReportRepository, CustomerReportRepository>();
        // Provincial
        services.AddTransient<ICarPaymentReportRepository, CarPaymentReportRepository>();
        services.AddTransient<ICarPaymentRepository, CarPaymentRepository>();

        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IGeneralRepository, GeneralRepository>();
        services.AddTransient<IWarehouseRepository, WarehouseRepository>();
        services.AddTransient<IItemRepository, ItemRepository>();
        services.AddTransient<IGeneratorRepository, GeneratorRepository>();
        // Inventory
        services.AddTransient<IInventoryRepository, InventoryRepository>();
        services.AddTransient<IProductRepository, ProductRepository>();
        services.AddTransient<IInventoryReportRepository, InventoryReportRepository>();
        services.AddTransient<IInventoryExpiredRepository, InventoryExpiredRepository>();
        services.AddTransient<IInventoryTrackingRepository, InventoryTrackingRepository>();
        services.AddTransient<IWarehousePresetRepository, WarehousePresetRepository>();
        services.AddTransient<IOpeningBalanceRepository, OpeningBalanceRepository>();
        services.AddTransient<IClosingInventoryRepository, ClosingInventoryRepository>();
        services.AddTransient<IItemSaleStockRepository, ItemSaleStockRepository>();
        services.AddTransient<IViewStockupRepository, ViewStocRepository>();
        services.AddTransient<IStockCountingRepository, StockCountingRepository>();
        services.AddTransient<ITemplateRepository, TemplateRepository>();
        services.AddTransient<IStockCarInvoiceRepository, StockCarInvoiceRepository>();
        services.AddTransient<ITransferMoneyRepository, TransferMoneyRepository>();
        services.AddTransient<IStockCarExpenseRepository, StockCarExpenseRepository>();
        services.AddTransient<IStockCarPaymentInvoiceRepository, StockCarPaymentInvoiceRepository>();


        // validate Schema
        services.AddValidatorsFromAssemblyContaining<AccReceivablePresetValidate>();
        services.AddValidatorsFromAssemblyContaining<ChangeInvoiceValidate>();
        services.AddValidatorsFromAssemblyContaining<DividedInvoiceValidate>();
        services.AddValidatorsFromAssemblyContaining<InvoiceClosingEntriesValidate>();
        services.AddValidatorsFromAssemblyContaining<PaymentInvoiceValidate>();
        services.AddValidatorsFromAssemblyContaining<MarketValidate>();
        services.AddValidatorsFromAssemblyContaining<OldInvoiceValidate>();
        services.AddValidatorsFromAssemblyContaining<UpdatePaymentInvoiceValidate>();
        services.AddValidatorsFromAssemblyContaining<ConfirmBalanceValidate>();
    }
}