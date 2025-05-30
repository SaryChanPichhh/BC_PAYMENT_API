

using System.Data;
using System.Diagnostics;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.APPLICATION.Interfaces.Generator;
using BC.PAYMENT.CORE.DTO.Generator;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Generator
{
    public class GeneratorRepository : IGeneratorRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly IDbConnection _dbConnection;
        private readonly IConfiguration _configuration;
        private const int MaxInvoiceNumber = 9999;
        public GeneratorRepository(ISqlDataAccess sqlDataAccess, IDbConnection dbConnection, IConfiguration configuration)
        {
            _sqlDataAccess = sqlDataAccess;
            _dbConnection = dbConnection;
            _configuration = configuration;
        }

        public async Task<SaleAnalysisDto> GetSaleAnalysisByCustomerCodeAsync(string customerCode, string dbCode)
        { 
            var sql =
                @"SELECT ANAL_C0 AnalysisC0,ANAL_C1 AnalysisC1,ANAL_C2 AnalysisC2,ANAL_C3 AnalysisC3,ANAL_C4 AnalysisC4,ANAL_C5 AnalysisC5,ANAL_C6 AnalysisC6,ANAL_C7 AnalysisC7,ANAL_C8 AnalysisC8,ANAL_C9 AnalysisC9
                FROM SIADDANAL WHERE ADD_CODE = @CustomerCode AND DB_CODE = @DbCode";
            var param = new
            {
                CustomerCode = customerCode,
                DbCode = dbCode
            };
            var results = await _sqlDataAccess.LoadSingleData<SaleAnalysisDto,dynamic>(sql, param);
            return results;
        }

        public async Task<string> GenerateAdjRefCode(string dbCode,string movType, string recType)
        {
            var sql =
                @$"SELECT MAX(RIGHT(MOV_REF,4)) FROM {dbCode}SIINVMOV 
            WHERE MOV_TYPE = @MOV_TYPE AND REC_TYPE = 'M' AND MONTH(MOV_DATE) = MONTH(GETDATE())";
            var param = new
            {
                DB_CODE = dbCode,
                MOV_TYPE = movType,
                REC_TYPE = recType
            };
            var results = await _sqlDataAccess.LoadSingleData<string,dynamic>(sql, param);
            var year = DateTime.Now.Year.ToString().Substring(2, 2);
            var month = DateTime.Now.Month.ToString("D2");
            var suffix = year + month;
            if (!string.IsNullOrEmpty(results) && results.All(char.IsDigit))
            {
                var numericResult = Convert.ToInt32(results);
                return numericResult != MaxInvoiceNumber
                    ? $"{movType}{suffix}{numericResult + 1:D4}"
                    : $"{movType}{suffix}0001";
            }
            return $"{movType}{suffix}0001";
        }

        public async Task<string> GenerateFixInvoice(string dbCode)
        {
            const string sql =
                @"SELECT MAX(SUBSTRING(INVOICE_NUMBER,7,4)) FROM TB_BC_CHANGEINVOICE_REPAIR_INVOICE 
            WHERE INVOICE_NUMBER LIKE 'FF%' AND DB_CODE = @DB_CODE AND LEN(INVOICE_NUMBER) = 10
            AND MONTH(CREATED_DATE) = MONTH(GETDATE())";
            var param = new
            {
                DB_CODE = dbCode
            };
            var results = await _sqlDataAccess.LoadSingleData<string, dynamic>(sql, param);
            var year = DateTime.Now.Year.ToString().Substring(2, 2);
            var month = DateTime.Now.Month.ToString("D2");
            var suffix = year + month;
            if (!string.IsNullOrEmpty(results))
            {
                return Convert.ToInt32(results) != MaxInvoiceNumber
                    // if max invoice number is not equal to max invoice number in database
                    ? $"FF{suffix}{Convert.ToInt32(results) + 1:D4}"
                    // if max invoice number is 9999, then generate new invoice number
                    : $"FO{suffix}0000";
            }
            // if max invoice number is empty, then generate new invoice number
            return $"FF{suffix}0000";
        }

        public async Task<string> PostSaleOrderAutoNumberAsync(string saleType, string dbCode)
        {
            const string sql =
                @"SELECT SI_DATA FROM SIDATA WHERE SI_TYPE = 'AUTON' AND CODE = @SaleType AND DB_CODE = @DB_CODE";
            var result =
                await _sqlDataAccess.LoadSingleData<string,dynamic>(sql,
                    new { DB_CODE = dbCode, SaleType = saleType });
            if (string.IsNullOrEmpty(result)) return result;
            var prefix = result.Substring(32, 10).Trim();
            var suffix = result.Substring(42, 10).Trim();
            var interval = result.Substring(52, 5);
            var length = result.Substring(57, 2).Trim();
            var start = result.Substring(59, 5).Trim();
            var prefixResult = string.IsNullOrEmpty(prefix) ? "" : Prefix(prefix);
            var suffixResult = string.IsNullOrEmpty(suffix) ? "" : Prefix(suffix);
            var generateId =
                GenerateId($"SELECT MAX(TRANS_REF) FROM dbo.{dbCode}SISOHDR WHERE REC_TYPE='O' AND " +
                           "LEFT(TRANS_REF," + prefixResult.Length + ")='" + prefixResult +
                           "' AND RIGHT(TRANS_REF," + suffixResult.Length + ")='"
                           + suffixResult + "'", int.Parse(length) -
                                                 (prefixResult.Length + suffixResult.Length),
                    prefixResult, suffixResult,
                    int.Parse(start), int.Parse(interval));
            GetId($"SELECT MAX(TRANS_REF) FROM dbo.{dbCode}SISOHDR WHERE REC_TYPE='O' AND " +
                  "LEFT(TRANS_REF," + prefixResult.Length + ")='" + prefixResult +
                  "' AND RIGHT(TRANS_REF," + suffixResult.Length + ")='"
                  + suffixResult + "'");
            return generateId;
        }

        public async Task<List<string>> GetSaleCodeAsync(string dbCode)
        {
            const string sql = @"SELECT CODE FROM SIDATA WHERE SI_TYPE = 'AUTON' AND DB_CODE = @DB_CODE";
            var param = new { DB_CODE = dbCode };
            var results = await _sqlDataAccess.LoadData<string,dynamic>(sql, param);
            return results.ToList();
        }

        private void GetId(string sqlStr)
        {
            if (_dbConnection.State == ConnectionState.Closed) _dbConnection.Open();

            var result = _dbConnection.Query(sqlStr, new { });


        }
        private string GenerateId(string sqlStr, int length, string preStr, string sufStr, int startN,
            int interval)
        {

            if (interval.ToString() == "")
                interval = 1;
            var connection = new SqlConnection(_dbConnection.ConnectionString);
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            var command = new SqlCommand(sqlStr, connection)
            {
                CommandTimeout = 0
            };
            var dataAdapter = new SqlDataAdapter(command);
            var dt = new DataTable();
            dataAdapter.Fill(dt);

            //foreach (DataRow row in dt.Rows)
            //{
            //    var rd = row[0].ToString();
            //}
            var formatStr = "0";
            for (var i = 1; i < length; i++) formatStr += "0";

            formatStr = "{0:" + formatStr + "}";
            var vv = command.ExecuteScalar();
            if (vv == null) return preStr + string.Format(formatStr, startN) + sufStr;
            decimal id = 0;
            var st = Convert.ToString(command.ExecuteScalar().ToString());
            if (string.IsNullOrEmpty(st)) st = "0";

            if (length == 0)
            {
                if (st.IsNumeric()) id = Convert.ToDecimal(st) + interval;
            }
            else
            {
                if (st.Length >= length && preStr.Length <= st.Length)
                {
                    if (!st.Substring(preStr.Length, length).IsNumeric())
                        return preStr + string.Format(formatStr, id) + sufStr;
                    _ = Convert.ToDecimal(st.Substring(preStr.Length, length));
                    id = Convert.ToDecimal(st.Substring(preStr.Length, length)) + interval;
                }
                else
                {
                    if (st.IsNumeric()) id = Convert.ToDecimal(st) + interval;
                }
            }

            Console.WriteLine(sqlStr);
            return preStr + string.Format(formatStr, id) + sufStr;
        }
        private static string Prefix(string prefix)
        {
            if (prefix.Contains("!Y!"))
            {
                prefix = prefix.Replace("!Y!", DateTime.Now.Year.ToString().Substring(3));
            }
            else if (prefix.Contains(@"!YY!!MM!"))
            {
                var year = prefix.Replace("!YY!", $"{DateTime.Now.Year.ToString().Substring(2)}");
                var monthAndYear = year.Replace("!MM!", "");
                year = year.Replace(monthAndYear, "");
                var month = year.Replace("!MM!", $"{DateTime.Now.Month:00}");
                prefix = monthAndYear + month;
            }
            else if (prefix.Contains("!YY!"))
            {
                prefix = prefix.Replace("!YY!", DateTime.Now.Year.ToString().Substring(2));
            }
            else if (prefix.Contains("!YYYY!"))
            {
                prefix = prefix.Replace("!YYYY!", DateTime.Now.Year.ToString());
            }

            if (prefix.Contains("!MM!"))
                prefix = prefix.Replace("!MM!", string.Format(DateTime.Now.Month.ToString(), "00"));
            return prefix;
        }
    }
}
