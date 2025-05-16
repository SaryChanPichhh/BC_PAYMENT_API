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
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Accounting;
using BC.PAYMENT.INFRASTRUCTURE.Repository.General;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Generator;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Invoice;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Login;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Payment;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Prepare.Account;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Prepare.EmployeeSchedule;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Prepare.Preset;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.DailyPayment.AccountReceivable;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.DailyPayment.DailyPayment;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.DailyPayment.DeliveryPaid;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.DailyPayment.DividingInvoices.DailyReport;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.Submitting.SubmittingInvoice;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddTransient<APPLICATION.Interfaces.Payment.IDividedInvoiceRepository, Repository.Payment.DividedInvoiceRepository>();
            services.AddTransient<IDeliveryRepository, DeliveryRepository>();
            services.AddTransient<IAreaRepository, AreaRepository>();
            services.AddTransient<IMarketRepository, MarketRepository>();
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


                    services.AddTransient<IUnitOfWork, UnitOfWork>();
                    services.AddTransient<IGeneratorRepository, GeneratorRepository>();
        }
    }
}