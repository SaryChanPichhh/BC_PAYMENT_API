using DevExpress.Data.Entity;

namespace BC.ACCOUNTING.REPORT.Services
{
    public class ConnectionStringInfo : IConnectionStringInfo
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }

        // Implementing the missing members required by IConnectionStringInfo
        public string RunTimeConnectionString
        {
            get { return ConnectionString; }
        }

        // Location property should use DataConnectionLocation enum, not string
        public DataConnectionLocation Location { get; set; }

        // ProviderName, typically "System.Data.SqlClient" or another provider
        public string ProviderName { get; set; }
    }
}
