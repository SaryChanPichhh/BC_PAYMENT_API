namespace BC.PAYMENT.CORE.Entities.Report.DailyPaymentReport.CreditInvoice;

public class CreditInvoiceReportModel
{
    public string Delivery { get; set; }
    public string CustomerCode { get; set; }
    public string CustomerName { get; set; }
    public string Market { get; set; }
    public string Area { get; set; }
    public string Store { get; set; }
    public string TransactionCode { get; set; }
    public double InvoiceValue { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public bool IsReturn { get; set; }
    public DateTime CreateDate { get; set; }

    public string InvoiceType =>
        Status switch
        {
            "O" => "វិក្កយប័ត្រចាស់",
            "C" => "វិក្កយប័ត្រដូរ",
            "N" => "វិក្កយប័ត្រថ្មី",
            _ => "មិនស្គាល់"
        };
}