namespace BC.PAYMENT.API.Models
{
    public class PagedResponse<T> : ApiResponse<T>
    {
        private int TotalCount { get; set; }
        private int PageSize { get; set; }
        private int CurrentPageNumber { get; set; }
        private int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        public PagedResponse(int totalCount, T T, int currentPage, int pageSize) : base()
        {
            TotalCount = totalCount;
            Result = T;
            CurrentPageNumber = currentPage;
            PageSize = pageSize;

            TotalPages = (int)Math.Ceiling((double)TotalCount / (double)PageSize);
            HasPreviousPage = CurrentPageNumber > 1;
            HasNextPage = CurrentPageNumber < TotalPages;
        }

    }

}
