using BC.PAYMENT.APPLICATION.Interfaces.Submit;
using BC.PAYMENT.CORE.Contracts.Request.Expense;
using BC.PAYMENT.CORE.Contracts.Response.Expense;
using BC.PAYMENT.CORE.Entities;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using BC.PAYMENT.SQL.Queries;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Submit;

public class SubmitExpenseRepository(ISqlDataAccess sqlDataAccess) : ISubmitExpenseRepository
{
    public async Task<int> AddNewSubmitExpense(BcSubmittedPaidDetail detail)
    {
        var param = new
        {
            PAID_DETAIL_ID = detail.PaidDetailId,
            DOLLAR = detail.Dollar,
            RIEL = detail.Riel,
            EXCHANGE = detail.Exchange,
            TOTAL = detail.Total,
            EXPENSE_RIEL = detail.ExpenseRiel,
            MONEY_BIAS = detail.MoneyBais,
            EXPENSE_DOLLAR = detail.ExpenseDollar,
            STATUS = detail.Status ? 1 : 0,
            DB_CODE = detail.DbCode,
            SUBMITTED_DATE = detail.SubmittedDate,
            SUBMITTED_BY = detail.SubmittedBy
        };

        return await sqlDataAccess.ExecuteAsync(ExpenseQueries.InsertSubmitExpense, param);
    }

    public async Task<List<SubmitExpenseDetailResponse>> GetSubmitExpenseDetailByDateAsync(string dbCode,
        DateTime fromDate, DateTime toDate)
    {
        var criteria = $@"WHERE D.STATUS = '1'
                AND P.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE AND P.CREATED_DATE BETWEEN @FROM_DATE AND @TO_DATE";
        var data = await sqlDataAccess.LoadData<SubmitExpenseDetailResponse, dynamic>
        (ExpenseQueries.GetSubmitExpenseDetail(criteria), new
        {
            FROM_DATE = fromDate,
            TO_DATE = toDate,
            DB_CODE = dbCode
        });
        return data.ToList();
    }

    public async Task<List<ApprovedSubmitExpenseDetailResponse>> GetApprovedExpenseDetailByDateAsync(string dbCode,
        DateTime fromDate, DateTime toDate)
    {
        var criteria = $@"WHERE D.DB_CODE = @DB_CODE AND P.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE 
            AND P.CREATED_DATE BETWEEN @FROM_DATE AND @TO_DATE;";
        var addReference = $@"INNER JOIN BCAPPROVAL_PAID_DETAIL A ON A.SUBMITTED_ID = D.SUBMITTED_ID";
        var addOnField =
            $@",A.APPROVAL_STATUS ApprovedStatus ,A.DESCRIPTION Description,A.APPROVAL_DATE ApprovedDate,A.APPROVAL_BY ApprovedBy";
        var data = await sqlDataAccess.LoadData<ApprovedSubmitExpenseDetailResponse, dynamic>
        (ExpenseQueries.GetSubmitExpenseDetail
                (criteria, addReference: addReference, addOnField: addOnField),
            new { DB_CODE = dbCode, FROM_DATE = fromDate, TO_DATE = toDate });
        return data.ToList();
    }

    public async Task<int> UpdateSubmitExpenseDescriptionAsync(UpdateSubmitExpenseDescriptionRequest request)
    {
        var param = new
        {
            DESC_EXP_1 = request.DescExp1,
            DESC_EXP_2 = request.DescExp2,
            DESC_EXP_3 = request.DescExp3,
            SUBMITTED_ID = request.SubmittedId
        };

        return await sqlDataAccess.ExecuteAsync(ExpenseQueries.UpdateSubmitExpenseDescription, param);
    }

    public async Task<int> DeleteSubmitExpenseAsync(int submittedId)
    {
        var param = new
        {
            SUBMITTED_ID = submittedId
        };

        return await sqlDataAccess.ExecuteAsync(ExpenseQueries.DeleteSubmitExpense, param);
    }
}