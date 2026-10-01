using System.ComponentModel.DataAnnotations;

namespace BC.PAYMENT.API.Models;

public class AuthenticateRequest
{
    [Required] public string Username { get; set; }

    [Required] public string Password { get; set; }

    [Required]
    [RegularExpression("^[a-zA-Z0-9_]{3}")]
    public string DbCode { get; set; }
}