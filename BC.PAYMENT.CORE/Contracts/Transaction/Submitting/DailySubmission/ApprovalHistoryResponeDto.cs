namespace BC.PAYMENT.CORE.DTO.Transaction.Submitting.DailySubmission
{
    public  class ApprovalHistoryResponeDto
    {
        public List<string> ApprovedInvoices { get; set; } = new();
        public List<string> RejectedInvoices { get; set; } = new ();
        public List<string> ReSubmittedInvoices { get; set; } = new ();
        public Dictionary<DateTime,string> PublicHolidays { get; set; } = new();
    }
}
