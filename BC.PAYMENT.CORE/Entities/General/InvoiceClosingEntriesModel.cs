namespace BC.PAYMENT.CORE.Entities.General
{
    public class InvoiceClosingEntriesModel : BasedEntity
    {
        public int Id { get; set; }
        public string  DbCode { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime? ClosingDate { get; set; }
        public string ClosingBy { get; set; }
        public bool IsActive { get; set; }
        public string StatusText => IsActive ? "កំពុងដំណើរការ..." : "រួចរាល់";
    }
}
