namespace BC.PAYMENT.CORE.Contracts.Payment;

public record InvoiceExpenseDTO
{
    [JsonIgnore] public int Id { get; set; }

    [JsonIgnore] public string? DbCode { get; set; }

    [Required] public string Name { get; set; }

    [Required] public string Description { get; set; }

    [Required] public int PaymentInvoiceHeaderId { get; set; }

    [Required] public double UnitPrice { get; set; }

    [JsonIgnore] public double UnitPriceCalculated => CurrencyType == "Riel" ? UnitPrice / ExchangeRate : UnitPrice;

    [Required] public string? CurrencyType { get; set; }

    [Required] public double ExchangeRate { get; set; }

    [Required] public int Quantity { get; set; }

    [Required] public double Total { get; set; }

    [JsonIgnore] public double TotalCalculated => UnitPriceCalculated * Quantity;

    [JsonIgnore] public string? CreatedBy { get; set; }

    [JsonIgnore] public string? DeliveryName { get; set; }

    [JsonIgnore] public DateTime CreatedDate { get; set; }
}