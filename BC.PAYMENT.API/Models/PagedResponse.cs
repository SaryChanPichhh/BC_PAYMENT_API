namespace BC.PAYMENT.API.Models
{
    public class PagedResponse<T> : ApiResponse<T>
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPageNumber { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        public PagedResponse(int totalCount, T T, int currentPage, int pageSize)
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
