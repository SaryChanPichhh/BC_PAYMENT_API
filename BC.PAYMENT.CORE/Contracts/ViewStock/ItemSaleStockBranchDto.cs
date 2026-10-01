using System.Text.RegularExpressions;

namespace BC.PAYMENT.CORE.DTO.ViewStock;

public sealed class ItemSaleStockBranchDto
{
    private static readonly Regex DbCodePattern =
        new("^[A-Za-z0-9]{1,3}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string? DbCode { get; set; }
    public string? Location { get; set; }
    public string? AreaId { get; set; }

    public void NormalizeAndValidate(string defaultDbCode)
    {
        if (string.IsNullOrWhiteSpace(DbCode)) DbCode = defaultDbCode;

        DbCode = DbCode?.Trim().ToUpperInvariant();
        Location = NormalizeOptional(Location)?.ToUpperInvariant();
        AreaId = NormalizeOptional(AreaId);

        if (string.IsNullOrWhiteSpace(DbCode))
            throw new ArgumentException("The database code is required.", nameof(DbCode));

        if (!DbCodePattern.IsMatch(DbCode))
            throw new ArgumentException($"Invalid database code '{DbCode}'. Only 1-3 letters or digits are allowed.",
                nameof(DbCode));
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}