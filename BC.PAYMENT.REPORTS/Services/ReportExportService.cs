using BC.PAYMENT.REPORTS.Models;
using DevExpress.XtraReports.UI;

namespace BC.PAYMENT.REPORTS.Services;

public class ReportExportService
{
    public byte[] ExportReportToBytes(XtraReport report, Export format)
    {
        using var stream = new MemoryStream();

        switch (format)
        {
            case Export.Pdf:
                report.ExportToPdf(stream);
                break;
            case Export.Excel:
                report.ExportToXlsx(stream);
                break;
            case Export.Word:
                report.ExportToDocx(stream);
                break;
            default:
                throw new InvalidOperationException("Unsupported export format.");
        }

        return stream.ToArray();
    }

    public (string ContentType, string FileExtension) GetExportMetadata(Export format)
    {
        return format switch
        {
            Export.Pdf => ("application/pdf", "pdf"),
            Export.Excel => ("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx"),
            Export.Word => ("application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx"),

            _ => ("application/octet-stream", "bin")
        };
    }

    //public byte[] ExportReportToBytes(XtraReport report, BC.ACCOUNTING.CORE.DTO.General.Export format)
    //{
    //    using var stream = new MemoryStream();

    //    switch (format)
    //    {
    //        case BC.ACCOUNTING.CORE.DTO.General.Export.Pdf:
    //            report.ExportToPdf(stream);
    //            break;
    //        case BC.ACCOUNTING.CORE.DTO.General.Export.Excel:
    //            report.ExportToXlsx(stream);
    //            break;
    //        case BC.ACCOUNTING.CORE.DTO.General.Export.Word:
    //            report.ExportToDocx(stream);
    //            break;
    //        default:
    //            throw new InvalidOperationException("Unsupported export format.");
    //    }

    //    return stream.ToArray();
    //}

    //public (string ContentType, string FileExtension) GetExportMetadata(BC.ACCOUNTING.CORE.DTO.General.Export format)
    //{
    //    return format switch
    //    {
    //        BC.ACCOUNTING.CORE.DTO.General.Export.Pdf => ("application/pdf", "pdf"),
    //        BC.ACCOUNTING.CORE.DTO.General.Export.Excel => ("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx"),
    //        BC.ACCOUNTING.CORE.DTO.General.Export.Word => ("application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx"),

    //        _ => ("application/octet-stream", "bin")
    //    };
    //}
}