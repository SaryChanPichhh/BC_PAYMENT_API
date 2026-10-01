namespace BC.PAYMENT.API.Helper;

public static class GlobalFormat
{
    public static string ClosingInventoryFormatDateTime(this DateTime dateTime)
    {
        return dateTime.ToString("MM/dd/yyyy");
    }

    public static string FormatDate(DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd");
    }

    public static string FormatCurrency(decimal amount)
    {
        return amount.ToString("C2");
    }

    public static string FormatPercentage(decimal percentage)
    {
        return percentage.ToString("P2");
    }
}