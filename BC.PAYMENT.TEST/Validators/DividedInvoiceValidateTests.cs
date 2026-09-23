using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Validate;
using FluentValidation.TestHelper;
using Xunit;

namespace BC.PAYMENT.TEST.Validators
{
    public class DividedInvoiceValidateTests
    {
        private readonly CreateDividedInvoiceValidate _createValidator = new();
        private readonly DeleteDividedInvoiceValidate _deleteValidator = new();

        [Fact]
        public void CreateDividedInvoiceValidate_ShouldPass_WhenRequestIsValid()
        {
            var model = new CreateDividedInvoiceRequest
            {
                DeliveryId = "DEL01",
                InvoiceId = 101,
                CreatedDate = DateTime.UtcNow
            };

            var result = _createValidator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void CreateDividedInvoiceValidate_ShouldFail_WhenDeliveryIdIsNullOrEmpty(string? deliveryId)
        {
            var model = new CreateDividedInvoiceRequest
            {
                DeliveryId = deliveryId!,
                InvoiceId = 101
            };

            var result = _createValidator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.DeliveryId)
                .WithErrorMessage("Delivery Id is required.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void CreateDividedInvoiceValidate_ShouldFail_WhenInvoiceIdIsZeroOrNegative(int invoiceId)
        {
            var model = new CreateDividedInvoiceRequest
            {
                DeliveryId = "DEL01",
                InvoiceId = invoiceId
            };

            var result = _createValidator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.InvoiceId)
                .WithErrorMessage("Invoice Id must be greater than 0.");
        }

        [Fact]
        public void DeleteDividedInvoiceValidate_ShouldPass_WhenRequestIsValid()
        {
            var model = new DeleteDividedInvoiceRequest
            {
                InvoiceId = 1,
                TransactionCode = "INV-001",
                DeliveryId = "DEL01",
                Note = "Cancel invoice"
            };

            var result = _deleteValidator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(null)]
        public void DeleteDividedInvoiceValidate_ShouldFail_WhenInvoiceIdIsNullOrEmpty(int invoiceId)
        {
            var model = new DeleteDividedInvoiceRequest
            {
                InvoiceId = invoiceId,
                TransactionCode = "INV-001",
                DeliveryId = "DEL01",
                Note = "Cancel invoice"
            };

            var result = _deleteValidator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.InvoiceId)
                .WithErrorMessage("Invoice Id is required.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void DeleteDividedInvoiceValidate_ShouldFail_WhenTransactionCodeIsNullOrEmpty(string? transactionCode)
        {
            var model = new DeleteDividedInvoiceRequest
            {
                InvoiceId = 1,
                TransactionCode = transactionCode!,
                DeliveryId = "DEL01",
                Note = "Cancel invoice"
            };

            var result = _deleteValidator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.TransactionCode)
                .WithErrorMessage("Transaction Code is required.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void DeleteDividedInvoiceValidate_ShouldFail_WhenDeliveryIdIsNullOrEmpty(string? deliveryId)
        {
            var model = new DeleteDividedInvoiceRequest
            {
                InvoiceId = 1,
                TransactionCode = "INV-001",
                DeliveryId = deliveryId!,
                Note = "Cancel invoice"
            };

            var result = _deleteValidator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.DeliveryId)
                .WithErrorMessage("Delivery Id is required.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void DeleteDividedInvoiceValidate_ShouldFail_WhenNoteIsNullOrEmpty(string? note)
        {
            var model = new DeleteDividedInvoiceRequest
            {
                InvoiceId = 1,
                TransactionCode = "INV-001",
                DeliveryId = "DEL01",
                Note = note!
            };

            var result = _deleteValidator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Note)
                .WithErrorMessage("Note is required.");
        }
    }
}
