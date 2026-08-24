namespace BC.PAYMENT.CORE.DTO.ViewStock;

public sealed class ItemSaleStockRequestDto
{
    public List<ItemSaleStockBranchDto> Branches { get; set; } = new();
    public int? Year { get; set; }
    public int? Month { get; set; }
    public int? MonthsBack { get; set; }
    public List<string> TransCode { get; set; } = new();
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public void ApplyDefaultCurrentMonth(DateTime currentBusinessDate)
    {
        if (Year.HasValue || Month.HasValue || FromDate.HasValue || ToDate.HasValue) return;

        Year = currentBusinessDate.Year;
        Month = currentBusinessDate.Month;
        MonthsBack ??= 1;
    }

    public void NormalizeAndValidate(string authenticatedDbCode)
    {
        if (string.IsNullOrWhiteSpace(authenticatedDbCode))
            throw new ArgumentException("The authenticated database code is required.", nameof(authenticatedDbCode));

        Branches = Branches?.Where(static branch => branch is not null).ToList() ?? new List<ItemSaleStockBranchDto>();

        if (Branches.Count == 0)
            Branches.Add(new ItemSaleStockBranchDto
            {
                DbCode = authenticatedDbCode
            });

        foreach (var branch in Branches) branch.NormalizeAndValidate(authenticatedDbCode);

        Branches = Branches
            .GroupBy(static branch => branch.DbCode, StringComparer.OrdinalIgnoreCase)
            .Select(static group => group.First())
            .ToList();

        TransCode = TransCode?
                        .Where(static code => !string.IsNullOrWhiteSpace(code))
                        .Select(static code => NormalizeTransCode(code))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList()
                    ?? new List<string>();

        ValidateDateFilters();
    }

    private static string NormalizeTransCode(string code)
    {
        return code.Trim().Trim(',', ';', '|').Trim().ToUpperInvariant();
    }

    private void ValidateDateFilters()
    {
        var hasAnyYearMonth = Year.HasValue || Month.HasValue;
        var hasCompleteYearMonth = Year.HasValue && Month.HasValue;
        var hasAnyDateRange = FromDate.HasValue || ToDate.HasValue;
        var hasCompleteDateRange = FromDate.HasValue && ToDate.HasValue;

        if (hasAnyYearMonth && hasAnyDateRange)
            throw new ArgumentException("Use either Year/Month or FromDate/ToDate, not both.");
        if (hasAnyYearMonth && !hasCompleteYearMonth)
            throw new ArgumentException("Year and Month must be supplied together.");
        if (hasAnyDateRange && !hasCompleteDateRange)
            throw new ArgumentException("FromDate and ToDate must be supplied together.");
        if (!hasCompleteYearMonth && !hasCompleteDateRange)
            throw new ArgumentException("Year/Month or FromDate/ToDate is required.");
        if (Year.HasValue && Year.Value is < 2000 or > 2100)
            throw new ArgumentOutOfRangeException(nameof(Year), "Year must be between 2000 and 2100.");
        if (Month.HasValue && Month.Value is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(Month), "Month must be between 1 and 12.");
        if (hasCompleteYearMonth)
        {
            MonthsBack ??= 1;
            if (MonthsBack <= 0 || MonthsBack > 60)
                throw new ArgumentOutOfRangeException(nameof(MonthsBack), "MonthsBack must be between 1 and 60.");
        }

        if (hasCompleteDateRange && MonthsBack.HasValue)
            throw new ArgumentException("MonthsBack is only valid with Year and Month.");

        if (FromDate.HasValue && ToDate.HasValue && FromDate.Value.Date > ToDate.Value.Date)
            throw new ArgumentException("FromDate cannot be greater than ToDate.");
    }
}