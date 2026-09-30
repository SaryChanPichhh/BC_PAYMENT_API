namespace BC.PAYMENT.CORE.Contracts.Response.User;

public class UserResponse
{
    public int UserId { get; set; }
    public string? Username { get; set; }
    public string? DbCode { get; set; }
    public List<BranchDTO> Branches { get; set; } = [];
    [JsonIgnore]
    public string? UserPass { get; set; }
    //public bool? UserStatus { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CompanyCode { get; set; } = string.Empty;
    public string AppCode { get; set; } = string.Empty;
    public DateTime CurrentDate { get; set; }
    public int RowNumber { get; set; }
    public string InvoiceEntryCode { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
}