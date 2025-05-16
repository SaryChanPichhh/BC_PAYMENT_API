using BC.PAYMENT.APPLICATION.Interfaces.Accounting;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.APPLICATION.Interfaces.Generator;
using BC.PAYMENT.APPLICATION.Interfaces.Invoice;
using BC.PAYMENT.APPLICATION.Interfaces.Login;
using BC.PAYMENT.APPLICATION.Interfaces.Payment;
using BC.PAYMENT.APPLICATION.Interfaces.Prepare.Account;
using BC.PAYMENT.APPLICATION.Interfaces.Prepare.EmployeeSchedule;
using BC.PAYMENT.APPLICATION.Interfaces.Prepare.Preset;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.AccountReceivable;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DailyPayment;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DeliveryPaid;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.DailyReport;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.Submitting.SubmittingInvoice;
using IDividedInvoiceRepository = BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.Invoice.IDividedInvoiceRepository;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IUserRepository users, IBranchRepository branches, ICustomerRepository customers, IInvoiceRepository invoices, IAnalysisAccountRepository analysisAccounts, APPLICATION.Interfaces.Payment.IDividedInvoiceRepository dividedInvoices, IDeliveryRepository deliveries, IMarketRepository markets, IDistrictRepository districts, IProvinceRepository provinces, IAccountReceivablePresetRepository accountReceivablePresets, IPublicHolidayRepository publicHoliday, INewInvoiceRepository newInvoice, IChangeInvoiceRepository changeInvoice, IReturnInvoiceRepository returnInvoice, IIssuanceInvoiceRepository issuanceInvoice, IDividedInvoiceRepository dividedInvoice, IInvoiceReportRepository invoiceReport, ICheckReturnInvoiceRepository checkReturnInvoice, IDeliveryPaidRepository deliveryPaid, IDailyPaymentRepository dailyPayment, IConfirmAccountReceivableRepository confirmAccountReceivable, ISubmittingInvoiceRepository submittingInvoice, IGeneratorRepository generators, IAccountReceivableRepository accountReceivable)
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
   
    }
}
