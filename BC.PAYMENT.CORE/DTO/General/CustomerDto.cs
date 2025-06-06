

using BC.PAYMENT.CORE.Entities.General;

namespace BC.PAYMENT.CORE.DTO.General
{
    public class CustomerDto : Customer
    {
        public int MasterId { get; set; }
        public int ReceivedId { get; set; }
        public int RequestDetailId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Quantity { get; set; }
        public string FullName => FirstName + " " + LastName;
        public string Phone { get; set; }
        public string Map { get; set; }
        public DateTime RequestDate { get; set; }
        public string UserCode { get; set; }
        public string Location => $@"{CustomerName} {Area} {Market}";
    }

    public class CustomerWhoRepairedGoodsDto
    {
        public string UserCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }


    }
    public class CustomerRespondDto : Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => FirstName + " " + LastName;
        public string UserCode { get; set; }
        public string Location => $@"{CustomerName} {Area} {Market}";
    }
}
