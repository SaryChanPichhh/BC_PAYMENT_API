namespace BC.PAYMENT.CORE.Entities.Report.OthersReport
{
    public class CustomerReportModel :Customer
    {
        public string DbCode { get; set; }
        public string DbName { get; set; }
        public string FullName { get; set; }
        public string TypeCheck { get; set; }
        public DateTime DateCheck { get; set; }
        public bool IsMet { get; set; }
        public string IsMetStatus => IsMet ? "បានជួប" : "មិនបានជួប";
        public bool IsOrdered { get; set; }
        public string IsOrderedStatus => IsOrdered ? "បានកម៉្មង់" : "មិនបានកម៉្មង់";
        public string Description { get; set; }
        public int UserId { get; set; }
        public bool Status { get; set; }
        public int CreateBy { get; set; }
        public string Distance { get; set; }
        public double Lat1 { get; set; }
        public double Lat2 { get; set; }
        public double Lon1 { get; set; }
        public double Lon2 { get; set; }
    }
}
