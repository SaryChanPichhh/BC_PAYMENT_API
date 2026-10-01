using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Validate;
using FluentValidation.TestHelper;
using Xunit;

namespace BC.PAYMENT.TEST.Validators;

public class PaymentInvoiceValidateTests
{
    private readonly PaymentInvoiceValidate _validator = new();

    [Fact]
    public void PaymentInvoiceValidate_ShouldPass_WhenRequestIsValid()
    {
        var model = new PaymentInvoiceRequest
        {
            DividedInvoiceId = 1,
            PaymentStatus = "Paid",
            Amount = 150.75,
            Description = "Full payment received"
        };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void PaymentInvoiceValidate_ShouldFail_WhenDividedInvoiceIdIsZeroOrNegative(int dividedInvoiceId)
    {
        var model = new PaymentInvoiceRequest
        {
            DividedInvoiceId = dividedInvoiceId,
            PaymentStatus = "Paid",
            Amount = 100,
            Description = "Payment note"
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.DividedInvoiceId)
            .WithErrorMessage("Divided Invoice Id must be greater than 0.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void PaymentInvoiceValidate_ShouldFail_WhenPaymentStatusIsNullOrEmpty(string? paymentStatus)
    {
        var model = new PaymentInvoiceRequest
        {
            DividedInvoiceId = 1,
            PaymentStatus = paymentStatus!,
            Amount = 100,
            Description = "Payment note"
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PaymentStatus)
            .WithErrorMessage("Payment Status is required.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [InlineData(-50)]
    public void PaymentInvoiceValidate_ShouldFail_WhenAmountIsZeroOrNegative(double amount)
    {
        var model = new PaymentInvoiceRequest
        {
            DividedInvoiceId = 1,
            PaymentStatus = "Paid",
            Amount = amount,
            Description = "Payment note"
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage("Amount must be greater than 0.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void PaymentInvoiceValidate_ShouldFail_WhenDescriptionIsNullOrEmpty(string? description)
    {
        var model = new PaymentInvoiceRequest
        {
            DividedInvoiceId = 1,
            PaymentStatus = "Paid",
            Amount = 100,
            Description = description!
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description is required.");
    }
}