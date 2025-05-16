
using BC.PAYMENT.APPLICATION.Interfaces.Accounting;
using BC.PAYMENT.CORE.Entities.Accounting;
using Microsoft.Data.SqlClient;
using static BC.PAYMENT.CORE.Entities.Accounting.AccountReceivableModel;
using System.Data;
using BC.PAYMENT.CORE.DTO.Generator;
using BC.PAYMENT.CORE.Enums;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using Dapper;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Accounting
{
    public class AccountReceivableRepository : IAccountReceivableRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ISqlDataAccess _sqlDataAccess;
        public AccountReceivableRepository(IDbConnection dbConnection, ISqlDataAccess sqlDataAccess)
        {
            _dbConnection = dbConnection;
            _sqlDataAccess = sqlDataAccess;
        }
        public async Task<List<AccountReceivablePatternModel>> GetAccountReceivablePatterns(string dbCode)
        {
            const string sql = @"SELECT 
        DATA_CODE AccountCode,
        DATA_NAME CreditDebitType,
        DATA_DESC Description,
        DATA_TYPE DataType,
        FIELD_1 Field1,
        FIELD_2 Field2,
        FIELD_3 Field3,
        FIELD_4 Field4,
        FIELD_5 Field5,
        FIELD_6 Field6,
        FIELD_7 Field7,
        FIELD_8 Field8,
        FIELD_9 Field9,
        CREATED_DATE CreatedDate,
        CREATED_BY CreatedBy
        FROM BCDATA WHERE DB_CODE = @DB_CODE AND DATA_TYPE = @DATA_TPYE";
            var param = new { DB_CODE = dbCode, DATA_TPYE = "ACCOUNT RECEIVABLE" };
            var results = await _sqlDataAccess.LoadData<AccountReceivablePatternModel, dynamic>(sql, param);
            return results.ToList();
        }

        public async Task<dynamic> GetJournalTypesByDbCode(string dbCode)
        {
            const string sql = @"SELECT JNLT_CODE,JNLT_DESC FROM SIJNLTYP WHERE DB_CODE = @DB_CODE";
            var param = new { DB_CODE = dbCode };
            var results = await _sqlDataAccess.LoadData<dynamic, dynamic>(sql, param);
            return results;
        }

        public async Task<int> GetJournalIdByDbCode(string dbCode)
        {
            var sql = $@"SELECT MAX(JRNAL_NO) FROM {dbCode}SILEDG";
            var results = await _sqlDataAccess.ExecuteScalarAsync<int, dynamic>(sql, new { });
            return results;
        }

        public async Task<int> GetAllocReferenceByDbCode(string dbCode)
        {
            var sql = $@"SELECT MAX(ALLOC_REF) FROM {dbCode}SILEDG";
            var results = await _sqlDataAccess.ExecuteScalarAsync<int, dynamic>(sql, new { });
            return results;
        }

        public async Task<bool> SplitAccountReceivable(string customerCode, string referenceNo, string dbCode, double currentAmount,
            double splitAmount)
        {
            var sql =
            @$"SELECT * FROM {dbCode}SILEDG WHERE REFERENCE = @REFERENCE AND ACC_CODE = @ACC_CODE AND ALLOCATION = ''";
            var param = new
            {
                ACC_CODE = customerCode,
                REFERENCE = referenceNo
            }; 
            // Check connection status
            if (_dbConnection.State != ConnectionState.Open)
            {
                 _dbConnection.Open();
            }

            // Get the current account receivable record from database
            var results = (await _dbConnection.QueryAsync<SILEDG>(sql, param)).ToList();
            // Check if the account receivable record is found or has been allocated
            if (results.Count is 0 or > 1)
            {
                return false;
            }

            // Get first record from the list
            var firstOrDefault = results.FirstOrDefault();
            // Check if first record is null
            if (firstOrDefault is null)
            {
                return false;
            }

            // Start transaction
            using var transaction = _dbConnection.BeginTransaction();
            var rowAffected = 0;
            try
            {
                // Delete the current account receivable record
                var sqlDelete =
                    @$"DELETE FROM {dbCode}SILEDG WHERE ACC_CODE = @ACC_CODE AND REFERENCE = @REFERENCE AND ALLOCATION = ''";
                var paramDelete = new
                {
                    ACC_CODE = customerCode,
                    REFERENCE = referenceNo
                };
                rowAffected += await _dbConnection.ExecuteAsync(sqlDelete, paramDelete, transaction);
                sql =
                    @$"INSERT INTO {dbCode}SILEDG (ACC_CODE, ACC_PERIOD, TRANS_DATE, JRNAL_NO, JRNAL_LINE, AMOUNT, D_C, JRNAL_TYPE, REFERENCE,
                       DESCRIPTN, ENTRY_DATE, ENTRY_PRD, DUE_DATE, ALLOCATION, ALLOC_REF, ALLOC_DATE, ALLOC_PERIOD,
                       ALLOC_USER, ASSET_CODE, ASSET_UPDT, CONV_CODE, CONV_SIGN, CONV_RATE, OTHER_AMT, LOSS_GAIN,
                       ANAL_T0, ANAL_T1, ANAL_T2, ANAL_T3, ANAL_T4, ANAL_T5, ANAL_T6, ANAL_T7, ANAL_T8, ANAL_T9,
                       TRAN_DESC1, TRAN_DESC2, TRAN_DESC3, TRAN_DESC4, TRAN_DESC5, TRAN_DESC6, ALLOC_IN_PROG, HOLD_REF,
                       HOLD_USER_CODE, USER_CREA, USER_UPDT, DATE_UPDT)
                VALUES (@ACC_CODE,@ACC_PERIOD,@TRANS_DATE,@JRNAL_NO,@JRNAL_LINE,@AMOUNT,@D_C,@JRNAL_TYPE,@REFERENCE,@DESCRIPTN,@ENTRY_DATE,@ENTRY_PRD,@DUE_DATE,@ALLOCATION,@ALLOC_REF,@ALLOC_DATE,@ALLOC_PERIOD,@ALLOC_USER,@ASSET_CODE,@ASSET_UPDT,@CONV_CODE,@CONV_SIGN,@CONV_RATE,@OTHER_AMT,@LOSS_GAIN,
                    @ANAL_T0,@ANAL_T1,@ANAL_T2,@ANAL_T3,@ANAL_T4,@ANAL_T5,@ANAL_T6,@ANAL_T7,@ANAL_T8,@ANAL_T9,@TRAN_DESC1,@TRAN_DESC2,@TRAN_DESC3,@TRAN_DESC4,@TRAN_DESC5,@TRAN_DESC6,@ALLOC_IN_PROG,
                    @HOLD_REF,@HOLD_USER_CODE,@USER_CREA,@USER_UPDT,@DATE_UPDT);";
                // Split the current account receivable record
                for (var i = 1; i <= 2; i++)
                {
                    if (i == 1)
                    {
                        firstOrDefault.AMOUNT = currentAmount;
                        var parameters = new DynamicParameters(firstOrDefault);
                        rowAffected += await _dbConnection.ExecuteAsync(sql, parameters, transaction);
                    }
                    else
                    {
                        var maxJournalLine = await _dbConnection.ExecuteScalarAsync<int>($@"SELECT MAX(JRNAL_LINE) FROM {dbCode}SILEDG WHERE REFERENCE = @REFERENCE AND ACC_CODE = @ACC_CODE", new { ACC_CODE = customerCode, REFERENCE = referenceNo }, transaction);
                        firstOrDefault.AMOUNT = splitAmount;
                        firstOrDefault.ALLOCATION = "A";
                        firstOrDefault.JRNAL_LINE = maxJournalLine + 1;
                        var parameters = new DynamicParameters(firstOrDefault);
                        rowAffected += await _dbConnection.ExecuteAsync(sql, parameters, transaction);
                    }
                }
                if (rowAffected == 3)
                {
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                }

                // return true if the account receivable record is successfully split
                return rowAffected == 3;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                transaction.Rollback();
                return false;
            }
        }

        public async Task<int> InsertAccountReceivable(AccountReceivableParameter model,bool isAccountsReceivableCompleted = false)
        {
            if(_dbConnection.State == ConnectionState.Closed) _dbConnection.Open();
            var results = await GetAccountReceivablePatterns(model.DbCode);
            if (results.Count != 2) return 0;
            var maxJournalId = await GetJournalIdByDbCode(model.DbCode);
            var maxAllocReference = await GetAllocReferenceByDbCode(model.DbCode);
            maxJournalId++;
            maxAllocReference++;
            var rowAffected = 0;
            var journalLine = 0;
            using var transaction = _dbConnection.BeginTransaction();
            var procedureName = $"{model.DbCode}SI_INSERT_SILEDG";
            foreach (var accountReceivablePatternModel in results)
            {
                journalLine++;
                if (accountReceivablePatternModel.CreditDebitType == CreditDebitType.Credit)
                {
                    var param = new
                    {
                        ACC_CODE_1 = model.ACC_CODE,
                        model.ACC_PERIOD_2,
                        model.TRANS_DATE_3,
                        JRNAL_NO_4 = maxJournalId,
                        JRNAL_LINE_5 = journalLine,
                        AMOUNT_6 = model.AMOUNT_6 * -1,
                        D_C_7 = "C",
                        JRNAL_TYPE_8 = accountReceivablePatternModel.JournalType,
                        model.REFERENCE_9,
                        model.DESCRIPTN_10,
                        model.ENTRY_DATE_11,
                        model.ENTRY_PRD_12,
                        model.DUE_DATE_13,
                        ALLOCATION_14 = "A",
                        ALLOC_REF_15 = maxAllocReference,
                        ALLOC_DATE_16 = DateTime.Today.ToString("MM/dd/yyyy"),
                        model.ALLOC_PERIOD_17,
                        model.ALLOC_USER_18,
                        model.ASSET_CODE_19,
                        ASSET_UPDT_20 = "N",
                        model.CONV_CODE_21,
                        model.CONV_SIGN_22,
                        CONV_RATE_23 = "0.00000",
                        OTHER_AMT_24 = "0.00000",
                        model.LOSS_GAIN_25,
                        model.ANAL_T0_26,
                        model.ANAL_T1_27,
                        model.ANAL_T2_28,
                        model.ANAL_T3_29,
                        model.ANAL_T4_30,
                        model.ANAL_T5_31,
                        model.ANAL_T6_32,
                        model.ANAL_T7_33,
                        model.ANAL_T8_34,
                        model.ANAL_T9_35,
                        model.TRAN_DESC1_36,
                        model.TRAN_DESC2_37,
                        model.TRAN_DESC3_38,
                        model.TRAN_DESC4_39,
                        model.TRAN_DESC5_40,
                        model.TRAN_DESC6_41,
                        model.ALLOC_IN_PROG_42,
                        HOLD_REF_43 = "0",
                        model.HOLD_USER_CODE_44,
                        model.USER_CREA_45,
                        model.USER_UPDT_46,
                        model.DATE_UPDT_47,
                    };
                    rowAffected +=
                        await _dbConnection.ExecuteAsync(procedureName, param, transaction, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    var param = new
                    {
                        ACC_CODE_1 = accountReceivablePatternModel.AccountCode,
                        model.ACC_PERIOD_2,
                        model.TRANS_DATE_3,
                        JRNAL_NO_4 = maxJournalId,
                        JRNAL_LINE_5 = journalLine,
                        model.AMOUNT_6,
                        D_C_7 = "D",
                        JRNAL_TYPE_8 = accountReceivablePatternModel.JournalType,
                        model.REFERENCE_9,
                        model.DESCRIPTN_10,
                        model.ENTRY_DATE_11,
                        model.ENTRY_PRD_12,
                        model.DUE_DATE_13,
                        ALLOCATION_14 = "",
                        model.ALLOC_REF_15,
                        ALLOC_DATE_16 = "",
                        ALLOC_PERIOD_17 = "0",
                        model.ALLOC_USER_18,
                        ASSET_CODE_19 = "",
                        ASSET_UPDT_20 = "N",
                        model.CONV_CODE_21,
                        model.CONV_SIGN_22,
                        model.CONV_RATE_23,
                        model.OTHER_AMT_24,
                        model.LOSS_GAIN_25,
                        model.ANAL_T0_26,
                        model.ANAL_T1_27,
                        model.ANAL_T2_28,
                        model.ANAL_T3_29,
                        model.ANAL_T4_30,
                        model.ANAL_T5_31,
                        model.ANAL_T6_32,
                        model.ANAL_T7_33,
                        model.ANAL_T8_34,
                        model.ANAL_T9_35,
                        model.TRAN_DESC1_36,
                        model.TRAN_DESC2_37,
                        model.TRAN_DESC3_38,
                        model.TRAN_DESC4_39,
                        model.TRAN_DESC5_40,
                        model.TRAN_DESC6_41,
                        model.ALLOC_IN_PROG_42,
                        model.HOLD_REF_43,
                        model.HOLD_USER_CODE_44,
                        model.USER_CREA_45,
                        model.USER_UPDT_46,
                        model.DATE_UPDT_47,
                    };
                    rowAffected +=
                        await _dbConnection.ExecuteAsync(procedureName, param, transaction, commandType: CommandType.StoredProcedure);
                }
            }
            if (isAccountsReceivableCompleted)
            {
                var sql =
                    @$"UPDATE {model.DbCode}SILEDG SET ALLOCATION=@ALLOCATION,ALLOC_REF=@ALLOC_REF,ALLOC_DATE=@ALLOC_DATE,ALLOC_PERIOD=@ALLOC_PERIOD WHERE ACC_CODE = @ACCOUNT_CODE AND REFERENCE = @REFERENCE AND ALLOCATION = ''";
                var paramUpdate = new
                {
                    ALLOCATION = "A",
                    ALLOC_REF = maxAllocReference,
                    ALLOC_DATE = DateTime.Today.ToString("MM/dd/yyyy"),
                    ALLOC_PERIOD = model.ALLOC_PERIOD_17,
                    ACCOUNT_CODE = model.ACC_CODE,
                    REFERENCE = model.REFERENCE_9
                };
                rowAffected += await _dbConnection.ExecuteAsync(sql, paramUpdate, transaction);
            }
            if (rowAffected is 2 or 3)
            {
                transaction.Commit();
            }
            else
            {
                transaction.Rollback();
            }

            return rowAffected ;
        }
    }
}
