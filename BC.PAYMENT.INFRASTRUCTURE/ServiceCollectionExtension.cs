using BC.PAYMENT.APPLICATION.Interfaces.ViewStock;
using BC.PAYMENT.INFRASTRUCTURE.Repository.ViewStock;
using DividedInvoiceRepository = BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.DailyPayment.DividingInvoices.Invoice.DividedInvoiceRepository;
using IDividedInvoiceRepository = BC.PAYMENT.APPLICATION.Interfaces.Payment.IDividedInvoiceRepository;

namespace BC.PAYMENT.INFRASTRUCTURE
{
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
            services.AddTransient<IDividedInvoiceRepository, Repository.Payment.DividedInvoiceRepository>();
            services.AddTransient<IDeliveryRepository, DeliveryRepository>();
            services.AddTransient<IAreaRepository, AreaRepository>();
            services.AddTransient<IMarketRepository, MarketRepository>();
            services.AddTransient<IEmployeeRepository, EmployeeRepository>();
            // Prepare 
                // Preset
                services.AddTransient<IDistrictRepository,DistrictRepository>();
                services.AddTransient<IProvinceRepository, ProvinceRepository>();
                services.AddTransient<IAccountReceivablePresetRepository, AccountReceivablePresetRepository>();
                services.AddTransient<IPublicHolidayRepository, PublicHolidayRepository>();
                // Preset
            // Prepare
            // Transaction
                // Dividing Invoices
                    services.AddTransient<INewInvoiceRepository, NewInvoiceRepository>();
                    services.AddTransient<IChangeInvoiceRepository, ChangeInvoiceRepository>();
                    services.AddTransient<APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.Invoice.IDividedInvoiceRepository, DividedInvoiceRepository>();
                    services.AddTransient<IInvoiceReportRepository, InvoiceReportRepository>();
                    services.AddTransient<IIssuanceInvoiceRepository, IssuanceRepository>();
                    services.AddTransient<INewInvoiceRepository, NewInvoiceRepository>();
                    services.AddTransient<IReturnInvoiceRepository, ReturnInvoiceRepository>();
                    services.AddTransient<ICheckReturnInvoiceRepository, CheckReturnInvoiceRepository>();
                // Delivery Paid
                    services.AddTransient<IDeliveryPaidRepository, DeliveryPaidRepository>();
                // Daily Payment
                    services.AddTransient<IDailyPaymentRepository, DailyPaymentRepository>();
                // Account Receivable
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
                    services.AddTransient<ICashFlowDataRepository,CashFlowDataRepository>();
                    services.AddTransient<ICashFlowDataReportRepository, CashFlowDataReportRepository>();
                    services.AddTransient<ICashFlowSubmittedRepository, CashFlowSubmittedRepository>();
                    services.AddTransient<ICashFlowAuditSubmittedRepository, CashFlowAuditSubmittedRepository>();
                    services.AddTransient<ICashFlowAuditReportRepository, CashFlowAuditReportRepository>();

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
                    services.AddTransient<IItemTransactionAnalysisRepository,ItemTransactionAnalysisRepository>();
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
                    services.AddTransient<ICarPaymentReportRepository,CarPaymentReportRepository>();
                    services.AddTransient<ICarPaymentRepository,CarPaymentRepository>();

                    services.AddTransient<IUnitOfWork, UnitOfWork>();
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
                    
            
            // validate Schema
            services.AddValidatorsFromAssemblyContaining<AccReceivablePresetValidate>();
        }
    }
}