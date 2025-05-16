namespace BC.PAYMENT.API.Models
{
    public class PaginatedResponse<T>
    {
        public List<T> Data { get; set; }
        public int TotalRecords { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public PaginatedResponse(List<T> data, int totalRecords, int page, int pageSize)
        {
            Data = data;
            TotalRecords = totalRecords;
            Page = page;
            PageSize = pageSize;
        }
    }

}
