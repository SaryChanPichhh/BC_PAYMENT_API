using BC.PAYMENT.REPORT.DataSources;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.WebDocumentViewer;

namespace BC.PAYMENT.REPORTS.Services;

public class CustomWebDocumentViewerReportResolver : IWebDocumentViewerReportResolver
{
    private readonly IServiceProvider _serviceProvider;

    public CustomWebDocumentViewerReportResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public XtraReport Resolve(string reportName)
    {
        throw new ArgumentException($"Unknown report name: {reportName}");
    }


    //public XtraReport Resolve(string reportEntry)
    //{
    //    if (reportEntry.StartsWith("EmployeeReport"))
    //    {
    //        XtraReport rep = CreateReport(reportEntry);
    //        rep.DataSource = CreateObjectDataSource(reportEntry);
    //        return rep;
    //    }
    //    return new XtraReport();
    //}

    private object CreateObjectDataSource(string reportName)
    {
        if (reportName == "EmployeeReport")
        {
            var dataSource = new ObjectDataSource();
            dataSource.Name = "EmployeeObjectDS";
            dataSource.DataSource = typeof(EmployeeList);
            dataSource.Constructor = ObjectConstructorInfo.Default;
            dataSource.DataMember = "Items";
            return dataSource;
        }
        else if (reportName.EndsWith("7"))
        {
            var dataSource = new ObjectDataSource();
            dataSource.Name = "EmployeeObjectDS";
            dataSource.DataSource = typeof(EmployeeList);
            // Specify the parameter's default value.
            var parameter = new Parameter("noOfItems", typeof(int), 7);
            dataSource.Constructor = new ObjectConstructorInfo(parameter);
            dataSource.DataMember = "Items";
            return dataSource;
        }
        else if (reportName.EndsWith("Parameter"))
        {
            var dataSource = new ObjectDataSource();
            dataSource.Name = "EmployeeObjectDS";
            dataSource.DataSource = typeof(EmployeeList);
            // Map data source parameter to report's parameter.
            var parameter = new Parameter
            {
                Name = "noOfItems",
                Type = typeof(DevExpress.DataAccess.Expression),
                Value = new DevExpress.DataAccess.Expression("?parameterNoOfItems", typeof(int))
            };
            dataSource.Constructor = new ObjectConstructorInfo(parameter);
            dataSource.DataMember = "Items";
            return dataSource;
        }
        else
        {
            var dataSource = new ObjectDataSource();
            dataSource.Name = "EmployeeObjectDS";
            dataSource.DataSource = typeof(EmployeeList);
            var parameterNoOfItems = new Parameter("noOfItems", typeof(int), 12);
            dataSource.Parameters.Add(parameterNoOfItems);
            dataSource.Constructor = ObjectConstructorInfo.Default;
            dataSource.DataMember = "GetData";
            return dataSource;
        }
    }

    private XtraReport CreateReport(string reportEntry)
    {
        return new XtraReport();
    }
}