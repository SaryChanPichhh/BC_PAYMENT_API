
using BC.PAYMENT.CORE.Entities.General;

namespace BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DeliveryPaid
{
    public class GeneralInvoicePaymentModel : GeneralInvoiceModel
    {
        public bool IsReturn { get; set; }
        public bool IsPaid { get; set; }
        public double? Total => InvoiceValue - PaidAmount;
        public string InvoiceStatus => IsReturn ? "ត្រឡប់" : IsPaid ? "ទូទាត់" : "ឥណទាន";
        public string InvoiceType => Status switch
        {
            "N" => "វិក័យប័ត្រថ្មី",
            "O" => "វិក័យប័ត្រចាស់",
            "C" => "វិក័យប័ត្រដូរ",
            _ => "មិនស្គាល់"
        };
    }

    public class GeneralInvoiceModel : Customer
    {
        public int DividedInvoiceId { get; set; }
        public DateTime Date { get; set; }
        public string? Delivery { get; set; }
        public string? DeliveryId { get; set; }
        public string? TransactionCode { get; set; }
        public double InvoiceValue { get; set; }
        public string? Description { get; set; }
        public double PaidAmount { get; set; }
        public string? Status { get; set; }
    }
    public class PaymentInvoiceHeaderModel
    {
        public int Id { get; set; }
        public string? DbCode { get; set; }
        public string? DeliveryId { get; set; }
        public int Period { get; set; }
        public DateTime InvoiceDividendDate { get; set; }
        public string? EntriesCode { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedDate { get; set; }
    }
    public class DeliveryGeneralInvoicePaidUpdateModel : GeneralInvoicePaymentModel
    {
        public double OldAmount { get; set; }
        public double NewAmount { get; set; }
        public int PaymentId { get; set; }
        public string? CreateBy { get; set; }
        public string? DbCode { get; set; }
        public double OldPaidAmount { get; set; }
        public double NewPaidAmount { get; set; }
    }

    public class DeliveryDataObject 
    {
        public string? DeliveryId { get; set; }
        public string? DeliveryName { get; set; }
        public string? ImagePath { get; set; }

        public class DeliveryImage
        {
            public byte[]? Image { get; set; }
        }
        public List<GeneralInvoicePaymentModel> InvoicePayments { get; set; } = new ();
    }
}
