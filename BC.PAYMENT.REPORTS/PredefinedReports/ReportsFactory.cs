using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;

namespace BC.PAYMENT.REPORT.PredefinedReports
{
    public static class ReportsFactory
    {
        public static Dictionary<string, Func<XtraReport>> Reports = new Dictionary<string, Func<XtraReport>>()
        {
            ["EmployeeReport"] = () => new PredefinedReports.EmployeeReport()
        };
    }
}
