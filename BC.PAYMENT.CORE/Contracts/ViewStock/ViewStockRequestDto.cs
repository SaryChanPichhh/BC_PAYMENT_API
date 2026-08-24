namespace BC.PAYMENT.CORE.DTO.ViewStock;

public sealed class ViewStockRequestDto
{
    public List<string> DbCodes { get; set; } = new();

    public List<string> NormalizeAndValidate()
    {
        var normalizedDbCodes = DbCodes?
                                    .Where(static dbCode => !string.IsNullOrWhiteSpace(dbCode))
                                    .Select(static dbCode => dbCode.Trim().ToUpperInvariant())
                                    .Distinct(StringComparer.OrdinalIgnoreCase)
                                    .ToList()
                                ?? new List<string>();

        if (normalizedDbCodes.Count == 0)
            throw new ArgumentException("At least one database code is required.", nameof(DbCodes));

        foreach (var dbCode in normalizedDbCodes)
            if (dbCode.Length is < 1 or > 3 ||
                !dbCode.All(char.IsLetterOrDigit))
                throw new ArgumentException(
                    $"Invalid database code '{dbCode}'. Only 1-3 letters or digits are allowed.", nameof(DbCodes));

        DbCodes = normalizedDbCodes;
        return normalizedDbCodes;
    }
}