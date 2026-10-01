namespace BC.PAYMENT.CORE;

public sealed class SqlConnectionString
{
    private static readonly Lazy<SqlConnectionString> _instance = new(() => new SqlConnectionString());

    public static SqlConnectionString Instance => _instance.Value;

    public string GetConnectionString { get; set; } = string.Empty;
}