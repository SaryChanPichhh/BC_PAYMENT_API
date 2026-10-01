namespace BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.ReviewReport;

public class RequestionActionDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public string Status => Name switch
    {
        "Pending" => "កំពុងរង់ចាំ",
        "Approve" => "បានអនុម័ត",
        _ => "បដិសេធ"
    };
}