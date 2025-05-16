namespace BC.PAYMENT.CORE.Entities.General
{
    public class InvoiceClosingEntriesModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime? ClosingDate { get; set; }
        public string ClosingBy { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string StatusText => IsActive ? "កំពុងដំណើរការ..." : "រួចរាល់";
    }
}
