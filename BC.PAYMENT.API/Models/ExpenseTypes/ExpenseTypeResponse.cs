using System;

namespace BC.PAYMENT.API.Models.ExpenseTypes;

public class ExpenseTypeResponse
{
    public string? ExpenseId { get; set; }
    public string? DbCode { get; set; }
    public string? ExpenseName { get; set; }
    public bool? Status { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
}