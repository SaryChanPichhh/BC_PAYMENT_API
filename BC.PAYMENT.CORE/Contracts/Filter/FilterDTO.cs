namespace BC.PAYMENT.CORE.DTO.Filter
{
    public record FilterDTO : BaseFilterDTO
    {

        [Required]
        public DateTime FromDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime ToDate { get; set; } = DateTime.Now;

    }
    public class PaginatedDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class PeriodPagedRequestDto 
    {
        public int FromPeriod { get; set; }
        public int ToPeriod { get; set; }
    }  
    public class DatePagedRequestDto 
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }   
    public class ByPeriodDto : PaginatedDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
    }
    public class ByDateDto : PaginatedDto
    {
        public string FromDate { get; set; } = string.Empty;
        public string ToDate { get; set; } = string.Empty;
    }
}
