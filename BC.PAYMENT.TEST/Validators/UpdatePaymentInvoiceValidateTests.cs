using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Validate;
using FluentValidation.TestHelper;
using Xunit;

namespace BC.PAYMENT.TEST.Validators
{
    public class UpdatePaymentInvoiceValidateTests
    {
        private readonly UpdatePaymentInvoiceValidate _validator = new();

        [Fact]
        public void UpdatePaymentInvoiceValidate_ShouldPass_WhenRequestIsValid()
        {
            var model = new UpdatePaymentInvoiceRequest
            {
                DividedId = 1,
                InvoiceId = 2,
                PaymentId = 3,
                OldAmount = 100.0,
                NewAmount = 150.0,
                Description = "Adjusted payment amount"
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void UpdatePaymentInvoiceValidate_ShouldFail_WhenDividedIdIsZeroOrNegative(int dividedId)
        {
            var model = new UpdatePaymentInvoiceRequest
            {
                DividedId = dividedId,
                InvoiceId = 1,
                PaymentId = 1,
                OldAmount = 100,
                NewAmount = 150,
                Description = "Note"
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.DividedId)
                .WithErrorMessage("Divided Id must be greater than 0.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-50)]
        public void UpdatePaymentInvoiceValidate_ShouldFail_WhenInvoiceIdIsZeroOrNegative(int invoiceId)
        {
            var model = new UpdatePaymentInvoiceRequest
            {
                DividedId = 1,
                InvoiceId = invoiceId,
                PaymentId = 1,
                OldAmount = 100,
                NewAmount = 150,
                Description = "Note"
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.InvoiceId)
                .WithErrorMessage("Invoice Id must be greater than 0.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-50)]
        public void UpdatePaymentInvoiceValidate_ShouldFail_WhenPaymentIdIsZeroOrNegative(int paymentId)
        {
            var model = new UpdatePaymentInvoiceRequest
            {
                DividedId = 1,
                InvoiceId = 1,
                PaymentId = paymentId,
                OldAmount = 100,
                NewAmount = 150,
                Description = "Note"
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.PaymentId)
                .WithErrorMessage("Payment Id must be greater than 0.");
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(-10)]
        public void UpdatePaymentInvoiceValidate_ShouldFail_WhenNewAmountIsNegative(double newAmount)
        {
            var model = new UpdatePaymentInvoiceRequest
            {
                DividedId = 1,
                InvoiceId = 1,
                PaymentId = 1,
                OldAmount = 100,
                NewAmount = newAmount,
                Description = "Note"
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.NewAmount)
                .WithErrorMessage("New Amount must be greater than or equal to 0.");
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(-10)]
        public void UpdatePaymentInvoiceValidate_ShouldFail_WhenOldAmountIsNegative(double oldAmount)
        {
            var model = new UpdatePaymentInvoiceRequest
            {
                DividedId = 1,
                InvoiceId = 1,
                PaymentId = 1,
                OldAmount = oldAmount,
                NewAmount = 100,
                Description = "Note"
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.OldAmount)
                .WithErrorMessage("Old Amount must be greater than or equal to 0.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void UpdatePaymentInvoiceValidate_ShouldFail_WhenDescriptionIsNullOrEmpty(string? description)
        {
            var model = new UpdatePaymentInvoiceRequest
            {
                DividedId = 1,
                InvoiceId = 1,
                PaymentId = 1,
                OldAmount = 100,
                NewAmount = 150,
                Description = description!
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Description is required.");
        }
    }
}
