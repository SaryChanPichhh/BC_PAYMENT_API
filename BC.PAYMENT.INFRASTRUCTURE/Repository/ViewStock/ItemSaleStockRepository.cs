using BC.PAYMENT.CORE.Entities.ViewStock;
using BC.PAYMENT.APPLICATION.Interfaces.ViewStock;
using BC.PAYMENT.CORE.DTO.ViewStock;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.ViewStock;

public sealed class ItemSaleStockRepository : IItemSaleStockRepository
{
    private const int MaximumMonthsBack = 60;
    private readonly ISqlDataAccess _sqlDataAccess;

    public ItemSaleStockRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess ?? throw new ArgumentNullException(nameof(sqlDataAccess));
    }

    public async Task<List<ItemSaleStockModel>> GetItemSaleStockAsync(ItemSaleStockRequestDto request, string? imageUrl,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var queryPeriod = BuildQueryPeriod(
            request.Year,
            request.Month,
            request.MonthsBack,
            request.FromDate,
            request.ToDate);

        var normalizedImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim().TrimEnd('/');

        var results = new List<ItemSaleStockModel>();

        foreach (var branch in request.Branches)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var resolvedTransCodes = await ResolveTransactionCodesAsync(
                branch.DbCode,
                request.TransCode,
                cancellationToken);

            if (request.TransCode.Count > 0 &&
                resolvedTransCodes.Count == 0)
                continue;

            var tableNames = CreateTableNames(branch.DbCode);
            var sql = BuildItemSaleStockSql(tableNames);

            var parameters = new
            {
                DB_CODE = branch.DbCode,
                MONTHS_BACK = queryPeriod.MonthsBack,
                IMAGE_URL = normalizedImageUrl,
                TRANS_CODE = resolvedTransCodes,
                TRANS_CODE_COUNT = resolvedTransCodes.Count,
                LOCATION = branch.Location,
                AREA_ID = branch.AreaId,
                FROM_DATE = queryPeriod.FromDate,
                TO_DATE_EXCLUSIVE = queryPeriod.ToDateExclusive
            };

            var branchRows = await _sqlDataAccess.LoadData<ItemSaleStockModel, dynamic>(sql, parameters);

            results.AddRange(branchRows);
        }

        return results
            .OrderBy(static item => item.DbCode, StringComparer.OrdinalIgnoreCase)
            .ThenByDescending(static item => item.SaleQty)
            .ThenBy(static item => item.ItemCode, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private async Task<List<string>> ResolveTransactionCodesAsync(string dbCode,
        IReadOnlyCollection<string> requestedTransCodes, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string sql =
            "SELECT S.CODE AS Code FROM dbo.SIDATA AS S WHERE S.DB_CODE = @DbCode AND S.SI_TYPE = 'SALES' AND S.SI_LOOKUP = 'A' ORDER BY S.CODE;";
        var rows = await _sqlDataAccess.LoadData<TransactionCodeModel, object>(sql, new { DbCode = dbCode });

        var activeCodes = rows
            .Where(static row => !string.IsNullOrWhiteSpace(row.Code))
            .Select(static row => row.Code!.Trim().ToUpperInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (activeCodes.Count == 0)
            throw new ArgumentException($"No active sales transaction codes were found for branch '{dbCode}'.");

        if (requestedTransCodes.Count == 0) return activeCodes;

        var resolvedCodes = activeCodes
            .Where(activeCode =>
                requestedTransCodes.Any(requestedCode => IsTransactionCodeMatch(activeCode, requestedCode)))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        return resolvedCodes;
    }

    private static bool IsTransactionCodeMatch(string activeCode, string requestedCode)
    {
        var normalizedRequestedCode = requestedCode.Trim().ToUpperInvariant();

        return activeCode.Equals(normalizedRequestedCode, StringComparison.OrdinalIgnoreCase) ||
               activeCode.StartsWith(normalizedRequestedCode + "-", StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildItemSaleStockSql(
        (string SaleDetailTable, string SaleHeaderTable, string StockMovementTable) tableNames)
    {
        return $"""
                SELECT I.DB_CODE AS DbCode,I.ITEM_CODE AS ItemCode,I.ITEM_DESC AS ItemDesc,ISNULL(B.SaleQty, 0) AS SaleQty,I.ITEM_CUS10_KH AS ItemNameKhmer,B.InvDate AS InvDate,ISNULL(STOCKS.StockQty, 0) AS AvailableQty,
                                CAST (
                                        CASE WHEN STOCKS.StockQty IS NULL THEN 0 ELSE 1 END AS BIT) AS StockStatus,
                                        CASE WHEN STOCKS.StockQty IS NULL THEN 'Unavailable In Stock' ELSE 'Available In Stock'
                                END AS StockStatusDesc,
                                CASE
                                    WHEN B.SaleQty > 0 AND B.SaleQty <= @MONTHS_BACK * 10 THEN N'Slow Moving'
                                    WHEN B.SaleQty > 0 AND B.SaleQty BETWEEN @MONTHS_BACK * 11 AND @MONTHS_BACK * 31 THEN N'Normal'
                                    WHEN B.SaleQty > 0 AND B.SaleQty BETWEEN @MONTHS_BACK * 32 AND @MONTHS_BACK * 100 THEN N'Fast Moving'
                                    WHEN B.SaleQty > 0 AND B.SaleQty > @MONTHS_BACK * 101 THEN N'Best Seller'
                                    WHEN ISNULL(B.SaleQty, 0) = 0 THEN N'Dead Stock'
                                END AS SaleStatus,
                                CASE
                                    WHEN @IMAGE_URL IS NULL OR I.IMG IS NULL OR LTRIM(RTRIM(I.IMG)) = '' THEN ''
                                    ELSE CONCAT(@IMAGE_URL, '/', I.IMG)
                                END AS ImageUrl
                            FROM dbo.SIITEMS I
                            INNER JOIN dbo.SIITEMANAL ANA ON ANA.ITEM_CODE = I.ITEM_CODE AND ANA.DB_CODE = @DB_CODE
                            LEFT JOIN(
                                SELECT A.ITEM_CODE, MAX(A.INV_DATE) AS InvDate, SUM(A.VALUE_1) AS SaleQty FROM dbo.{tableNames.SaleDetailTable} A
                                INNER JOIN dbo.{tableNames.SaleHeaderTable} B ON B.TRANS_REF = A.TRANS_REF
                                INNER JOIN dbo.SIWAREH E ON E.WAR_CODE = A.LOCATION AND E.DB_CODE = @DB_CODE
                                WHERE A.REC_TYPE = 'D' AND A.STATUS = '80' AND A.INV_DATE >= @FROM_DATE AND A.INV_DATE < @TO_DATE_EXCLUSIVE AND (@TRANS_CODE_COUNT = 0 OR B.TRANS_CODE IN @TRANS_CODE) AND (@LOCATION IS NULL OR A.LOCATION = @LOCATION) AND B.REC_TYPE = 'I'
                                GROUP BY A.ITEM_CODE) B ON B.ITEM_CODE = I.ITEM_CODE
                            LEFT JOIN
                            (
                                SELECT BS.ITEM_CODE, SUM(BS.QUANTITY) AS StockQty FROM dbo.{tableNames.StockMovementTable} BS WHERE BS.ALLOC_REF = '' GROUP BY BS.ITEM_CODE
                            ) STOCKS ON STOCKS.ITEM_CODE = I.ITEM_CODE
                            WHERE I.DB_CODE = @DB_CODE AND I.ITEM_STAT = 'A'
                              AND (@AREA_ID IS NULL OR EXISTS (SELECT 1 FROM dbo.TB_AREAS C WHERE C.DB_CODE = @DB_CODE AND C.ANAL_M0 = @AREA_ID))
                              AND (STOCKS.StockQty IS NOT NULL OR ISNULL(B.SaleQty, 0) <> 0)
                            ORDER BY SaleQty DESC
                            OPTION (RECOMPILE);
                """;
    }

    private static (DateTime FromDate, DateTime ToDateExclusive, int MonthsBack) BuildQueryPeriod(int? year, int? month,
        int? monthsBack, DateTime? fromDate, DateTime? toDate)
    {
        if (year.HasValue && month.HasValue)
        {
            var effectiveMonthsBack = monthsBack ?? 1;
            ValidateMonthsBack(effectiveMonthsBack);

            var selectedMonthStart = new DateTime(year.Value, month.Value, 1);

            return (selectedMonthStart.AddMonths(-(effectiveMonthsBack - 1)), selectedMonthStart.AddMonths(1),
                effectiveMonthsBack);
        }

        if (fromDate.HasValue && toDate.HasValue)
        {
            var normalizedFromDate = fromDate.Value.Date;
            var normalizedToDate = toDate.Value.Date;
            var effectiveMonthsBack = (normalizedToDate.Year - normalizedFromDate.Year) * 12 + normalizedToDate.Month -
                normalizedFromDate.Month + 1;
            ValidateMonthsBack(effectiveMonthsBack);

            return (normalizedFromDate, normalizedToDate.AddDays(1), effectiveMonthsBack);
        }

        throw new ArgumentException("Year/Month or FromDate/ToDate is required.");
    }

    private static void ValidateMonthsBack(int monthsBack)
    {
        if (monthsBack <= 0 || monthsBack > MaximumMonthsBack)
            throw new ArgumentOutOfRangeException(nameof(monthsBack),
                $"MonthsBack must be between 1 and {MaximumMonthsBack}.");
    }

    private static (string SaleDetailTable, string SaleHeaderTable, string StockMovementTable) CreateTableNames(
        string dbCode)
    {
        return ($"[{dbCode}SISODET]", $"[{dbCode}SISOHDR]", $"[{dbCode}SIINVMOV]");
    }

    private sealed class TransactionCodeModel
    {
        public string? Code { get; set; }
    }
}