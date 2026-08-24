namespace BC.ACCOUNTING.CORE.DTO.General
{
    public class ReportDTO
    {
        [Browsable(false)] public required string ReportName { get; set; }
        [Browsable(false)] public Export? ExportFormat { get; set; } = null; // null = View, otherwise Export
        [Browsable(false)] public string Connection { get; set; } = "Default";
    }

    public enum Export
    {
        Pdf = 1,
        Excel = 2,
        Word = 3
    }

}
