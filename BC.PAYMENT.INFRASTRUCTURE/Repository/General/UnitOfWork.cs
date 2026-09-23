using BC.PAYMENT.APPLICATION.Interfaces.ConfirmBalance;
using BC.PAYMENT.APPLICATION.Interfaces.StockCar;
using BC.PAYMENT.APPLICATION.Interfaces.Submit;
using IDividedInvoiceRepository = BC.PAYMENT.APPLICATION.Interfaces.Invoice.IDividedInvoiceRepository;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General
{
    public class UnitOfWork : IUnitOfWork
    {
        private IItemRepairReportRepository _itemRepairReport;

        public UnitOfWork(IUserRepository users, IBranchRepository branches, ICustomerRepository customers, IInvoiceRepository invoices, IAnalysisAccountRepository analysisAccounts, APPLICATION.Interfaces.Payment.IDividedInvoiceRepository dividedInvoices, IDeliveryRepository deliveries, IMarketRepository markets, IDistrictRepository districts, IProvinceRepository provinces, IAccountReceivablePresetRepository accountReceivablePresets, IPublicHolidayRepository publicHoliday, INewInvoiceRepository newInvoice, IChangeInvoiceRepository changeInvoice, IReturnInvoiceRepository returnInvoice, IDividedInvoiceRepository dividedInvoice, ICheckReturnInvoiceRepository checkReturnInvoice, IDeliveryPaidRepository deliveryPaid, IConfirmBalanceRepository confirmBalance, IConfirmAccountReceivableRepository confirmAccountReceivable, ISubmittingInvoiceRepository submittingInvoice, IGeneratorRepository generators, IAccountReceivableRepository accountReceivable, ISubmittingPerDeliveryRepository submittingPerDelivery, ISubmittedInvoiceRepository submittedInvoice, ISubmittedPerDeliveryRepository submittedPerDelivery, ISubmittedRejectedInvoiceRepository submittedRejectedInvoice, ISubmittedRejectedInvoicePerDeliveryRepository rejectedInvoicePerDelivery, IMonthlyInvoiceRepository invoiceVerify, ISubmissionHistoryRepository historyApproval, IDailySubmissionRepository approve, IStockInventoryCountingRepository stockInventoryCounting, IVerifyInvoiceRepository verifyInvoice, ISaleRepresentRepository saleRepresent, IReviewReportRepository reviewReport, IVerificationStockRepository verificationStock, ICheckingStockCarPaymentRepository checkingStockCarPayment, IVerificationRFIDRepository verificationRfid, IWarehouseRepository warehouses, IItemRepository items, ICashFlowDataRepository cashFlowData, ICashFlowDataReportRepository cashFlowDataReport, ICashFlowSubmittedRepository cashFlowSubmitted, ICashFlowAuditSubmittedRepository cashFlowAuditSubmitted, ICashFlowAuditReportRepository cashFlowAuditReport, IDailyRefundItemRepository dailyRefundItem, IRepairGoodsRepository repairGoods, IRepairerRepository repairer, ICompletedRepairRepository completedRepair, ICheckingInvoiceRepository checkingInvoice, IExchangeItemRepository exchangeItem, IItemTransactionAnalysisRepository itemTransactionAnalysis, IExchangeItemAnalysisRepository exchangeItemAnalysis, IAnnualPurchaseRepository annualPurchase, IOwedInvoiceRepository owedInvoice, IInventoryValueRepository inventoryValue, IDailyAnalysisRepository dailyAnalysis, ICreditInvoiceRepository creditInvoice, IPaidInvoiceRepository paidInvoice, ISummaryInvoiceRepository summaryInvoice, IAmountCollectedRepository amountCollected, IExpenseInvoiceReportRepository expenseInvoice, ITotalMonthlyPaymentRepository totalMonthlyPayment, IBillsOwedRepository billsOwed, ICustomerReportRepository customerReport, IEmployeeRepository employee, ICarPaymentReportRepository carPaymentReport, ICarPaymentRepository carPayment, IReportDividedInvoiceRepository reportDividedInvoice, IItemRepairReportRepository itemRepairReport, ICreditNoteReportRepository creditNoteReport, IItemExchangeReportRepository itemExchangeReport, IInventoryRepository inventory, IProductRepository products, IInventoryReportRepository inventoryReport, IInventoryExpiredRepository inventoryExpired, IInventoryTrackingRepository inventoryTracking, IWarehousePresetRepository warehousePreset, IOpeningBalanceRepository openingBalance, IStockCountingRepository stockCounting, IInvoiceClosingEntryRepository invoiceClosingEntry, IAreaRepository areas, IExpenseTypeRepository expenseTypes, IGeneralRepository generalRepository, IOldInvoiceRepository oldInvoice, IPaymentInvoiceRepository paymentInvoice, IExpenseRepository expense, IStockCarInvoiceRepository stockCarInvoice, ITemplateRepository template, ISubmitExpenseRepository? submitExpense = null, ITransferMoneyRepository? transferMoney = null, IStockCarExpenseRepository? stockCarExpense = null, IStockCarPaymentInvoiceRepository? stockCarPaymentInvoice = null)
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
            DividedInvoice = dividedInvoice;
            CheckReturnInvoice = checkReturnInvoice;
            DeliveryPaid = deliveryPaid;
            ConfirmBalance = confirmBalance;
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
            CheckingInvoice = checkingInvoice;
            ExchangeItem = exchangeItem;
            ItemTransactionAnalysis = itemTransactionAnalysis;
            ExchangeItemAnalysis = exchangeItemAnalysis;
            AnnualPurchase = annualPurchase;
            OwedInvoice = owedInvoice;
            InventoryValue = inventoryValue;
            DailyAnalysis = dailyAnalysis;
            CreditInvoice = creditInvoice;
            PaidInvoice = paidInvoice;
            SummaryInvoice = summaryInvoice;
            AmountCollected = amountCollected;
            ExpenseInvoice = expenseInvoice;
            TotalMonthlyPayment = totalMonthlyPayment;
            this.billsOwed = billsOwed;
            CustomerReport = customerReport;
            Employee = employee;
            CarPaymentReport = carPaymentReport;
            CarPayment = carPayment;
            ReportDividedInvoice = reportDividedInvoice;
            ItemRepairReport = itemRepairReport;
            CreditNoteReport = creditNoteReport;
            ItemExchangeReport = itemExchangeReport;
            Inventory = inventory;
            Products = products;
            InventoryReport = inventoryReport;
            InventoryExpired = inventoryExpired;
            InventoryTracking = inventoryTracking;
            WarehousePreset = warehousePreset;
            OpeningBalance = openingBalance;
            StockCounting = stockCounting;
            InvoiceClosingEntry = invoiceClosingEntry;
            Areas = areas;
            ExpenseTypes = expenseTypes;
            GeneralRepository = generalRepository;
            OldInvoice = oldInvoice;
            PaymentInvoice = paymentInvoice;
            Expense = expense;
            StockCarInvoice = stockCarInvoice;
            Template = template;
            TransferMoney = transferMoney!;
            StockCarExpense = stockCarExpense!;
            StockCarPaymentInvoice = stockCarPaymentInvoice!;
            SubmitExpense = submitExpense!;
        }


        public IGeneratorRepository Generators { get; }
        public IGeneralRepository GeneralRepository { get; }
        public IUserRepository Users { get; set; }
        public IBranchRepository Branches { get; set; }
        public ICustomerRepository Customers { get; set; }
        public IInvoiceRepository Invoices { get; set; }
        public IAnalysisAccountRepository AnalysisAccounts { get; set; }
        public APPLICATION.Interfaces.Payment.IDividedInvoiceRepository DividedInvoices { get; }
        public IDeliveryRepository Deliveries { get; }
        public IMarketRepository Markets { get; }
        public IAreaRepository Areas { get; }
        public IWarehouseRepository Warehouses { get; }
        public IItemRepository Items { get; }
        public IEmployeeRepository Employee { get; }
        public IInvoiceClosingEntryRepository InvoiceClosingEntry { get; }
        public IDistrictRepository Districts { get; set; }
        public IProvinceRepository Provinces { get; }
        public IAccountReceivablePresetRepository AccountReceivablePresets { get; }
        public IPublicHolidayRepository PublicHoliday { get; }
        public INewInvoiceRepository NewInvoice { get; }
        public IChangeInvoiceRepository ChangeInvoice { get; }
        public IReturnInvoiceRepository ReturnInvoice { get; }
        public IDividedInvoiceRepository DividedInvoice { get; }
        public ICheckReturnInvoiceRepository CheckReturnInvoice { get; }
        public IDeliveryPaidRepository DeliveryPaid { get; }
        public IConfirmBalanceRepository ConfirmBalance { get; }
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
        public ISubmitExpenseRepository SubmitExpense { get; }
        public IStockInventoryCountingRepository StockInventoryCounting { get; }
        public IVerifyInvoiceRepository VerifyInvoice { get; }
        public IStockCarInvoiceRepository StockCarInvoice { get; }
        public ITemplateRepository Template { get; }
        public ITransferMoneyRepository TransferMoney { get; }
        public IStockCarExpenseRepository StockCarExpense { get; }
        public IStockCarPaymentInvoiceRepository StockCarPaymentInvoice { get; }
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
        public ICheckingInvoiceRepository CheckingInvoice { get; }
        public IExchangeItemRepository ExchangeItem { get; }
        public IReportDividedInvoiceRepository ReportDividedInvoice { get; }
        public IItemExchangeReportRepository ItemExchangeReport { get; }
        public ICreditNoteReportRepository CreditNoteReport { get; }
        public IItemRepairReportRepository ItemRepairReport { get; }
        public IItemTransactionAnalysisRepository ItemTransactionAnalysis { get; }
        public IExchangeItemAnalysisRepository ExchangeItemAnalysis { get; }
        public IAnnualPurchaseRepository AnnualPurchase { get; }
        public IOwedInvoiceRepository OwedInvoice { get; }
        public IInventoryValueRepository InventoryValue { get; }
        public IDailyAnalysisRepository DailyAnalysis { get; }
        public ICreditInvoiceRepository CreditInvoice { get; }
        public IPaidInvoiceRepository PaidInvoice { get; }
        public ISummaryInvoiceRepository SummaryInvoice { get; }
        public IAmountCollectedRepository AmountCollected { get; }
        public IExpenseInvoiceReportRepository ExpenseInvoice { get; }
        public ITotalMonthlyPaymentRepository TotalMonthlyPayment { get; }
        public IBillsOwedRepository billsOwed { get; }
        public ICustomerReportRepository CustomerReport { get; }
        public ICarPaymentReportRepository CarPaymentReport { get; }
        public ICarPaymentRepository CarPayment { get; }
        public IInventoryRepository Inventory { get; }
        public IProductRepository Products { get; }
        public IInventoryReportRepository InventoryReport { get; }
        public IInventoryExpiredRepository InventoryExpired { get; }
        public IInventoryTrackingRepository InventoryTracking { get; }
        public IWarehousePresetRepository WarehousePreset { get; }
        public IOpeningBalanceRepository OpeningBalance { get; }
        public IStockCountingRepository StockCounting { get; }
        public IOldInvoiceRepository OldInvoice { get; }
        public IPaymentInvoiceRepository PaymentInvoice { get; }
        public IExpenseTypeRepository ExpenseTypes { get; }
        public IExpenseRepository Expense { get; }
        public IExpenseRepository Expenses => Expense;
        public IExpenseRepository ExpenseRepository => Expense;
    }
}
