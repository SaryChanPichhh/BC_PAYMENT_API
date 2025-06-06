

using System.ComponentModel.DataAnnotations;

namespace BC.PAYMENT.CORE.DTO.Preset.DailySaleAnalysis
{
    public class DailySaleAnalysisDto
    {
        
    }

    public class DailySaleAnalysisFilterByPeriodDto
    {
        [Required] public int FromPeriod { get; set; }
        [Required] public int ToPeriod { get; set; }
        [Required] public List<string> ItemCodes { get; set; }
    }
    public class DailySaleAnalysisFilterByDateDto
    {
        [Required] public string FromDate { get; set; }
        [Required] public string ToDate { get; set; }
        [Required] public List<string> ItemCodes { get; set; }
    }
}
