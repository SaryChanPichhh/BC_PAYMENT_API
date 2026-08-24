using System;
using DevExpress.Data.Entity;
using DevExpress.DataAccess.Web;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace BC.ACCOUNTING.REPORT.Services
{
    public class CustomSqlDataSourceProvider : IConnectionStringsProvider
    {
        private readonly IConfiguration _configuration;

        public CustomSqlDataSourceProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Shown in the Web Designer's data source dropdown
        public Dictionary<string, string> GetConnectionDescriptions()
        {
            return new Dictionary<string, string>
            {
                { "MSSQL_DataSource", "SQL Server (MyDatabase)" }
            };
        }

        // Used to retrieve the raw connection string
        public IConnectionStringInfo[] GetConnections()
        {
            // Returning the connection info using the updated ConnectionStringInfo class
            return new IConnectionStringInfo[]
            {
                new ConnectionStringInfo
                {
                    Name = "MSSQL_DataSource",
                    ConnectionString = _configuration.GetConnectionString("MSSQL_DataSource"),
                    Location = DataConnectionLocation.SettingsFile, // Enum value
                    ProviderName = "System.Data.SqlClient"
                }
            };
        }

        public IConnectionStringInfo[] GetConfigFileConnections()
        {
            return new IConnectionStringInfo[]
            {
                new ConnectionStringInfo
                {
                    Name = "MSSQL_DataSource",
                    ConnectionString = _configuration.GetConnectionString("MSSQL_DataSource"),
                    Location = DataConnectionLocation.SettingsFile, // Enum value
                    ProviderName = "System.Data.SqlClient"
                }
            };
        }

        public IConnectionStringInfo GetConnectionStringInfo(string connectionStringName)
        {
            if (connectionStringName == "MSSQL_DataSource")
            {
                return new ConnectionStringInfo
                {
                    Name = "MSSQL_DataSource",
                    ConnectionString = _configuration.GetConnectionString("MSSQL_DataSource"),
                    Location = DataConnectionLocation.SettingsFile, // Enum value
                    ProviderName = "System.Data.SqlClient"
                };
            }
            return null;
        }

        public string GetConnectionString(string name)
        {
            try
            {
                return _configuration.GetConnectionString(name);
            }
            catch (Exception ex)
            {
                throw new Exception($"Connection string error: {ex.Message}", ex);
            }
        }
    }
}
