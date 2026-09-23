using BC.PAYMENT.API.Controllers.Invoice;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.APPLICATION.Interfaces.Invoice;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Paid;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace BC.PAYMENT.TEST.Controllers
{
    public class PaymentInvoiceControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IInvoiceRepository> _mockInvoiceRepo;
        private readonly Mock<IPaymentInvoiceRepository> _mockPaymentInvoiceRepo;
        private readonly PaymentInvoiceController _controller;

        public PaymentInvoiceControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockInvoiceRepo = new Mock<IInvoiceRepository>();
            _mockPaymentInvoiceRepo = new Mock<IPaymentInvoiceRepository>();

            _mockUnitOfWork.Setup(u => u.Invoices).Returns(_mockInvoiceRepo.Object);
            _mockUnitOfWork.Setup(u => u.PaymentInvoice).Returns(_mockPaymentInvoiceRepo.Object);

            _controller = new PaymentInvoiceController(_mockUnitOfWork.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("DbCode", "TEST_DB"),
                new Claim("Username", "TEST_USER")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task SavePaymentInvoiceAsync_ShouldReturnOk_WhenSuccessful()
        {
            var request = new List<PaymentInvoiceRequest>
            {
                new()
                {
                    DividedInvoiceId = 1,
                    PaymentStatus = "Paid",
                    Amount = 100.0,
                    Description = "Payment for invoice 1"
                }
            };

            _mockInvoiceRepo.Setup(r => r.SavePaymentInvoiceAsync(request))
                .ReturnsAsync(1);

            var result = await _controller.SavePaymentInvoiceAsync(request);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Success);
            Assert.Equal(1, result.Result);
            Assert.Equal("Payment invoice saved successfully.", result.Message);
        }

        [Fact]
        public async Task SavePaymentInvoiceAsync_ShouldReturnBadRequest_WhenRowsAffectedIsZeroOrNegative()
        {
            var request = new List<PaymentInvoiceRequest>
            {
                new()
                {
                    DividedInvoiceId = 1,
                    PaymentStatus = "Paid",
                    Amount = 100.0,
                    Description = "Payment for invoice 1"
                }
            };

            _mockInvoiceRepo.Setup(r => r.SavePaymentInvoiceAsync(request))
                .ReturnsAsync(0);

            var result = await _controller.SavePaymentInvoiceAsync(request);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Payment invoice saved unsuccessfully.", result.Message);
        }

        [Fact]
        public async Task SavePaymentInvoiceAsync_ShouldReturnInternalServerError_WhenExceptionThrown()
        {
            var request = new List<PaymentInvoiceRequest>
            {
                new()
                {
                    DividedInvoiceId = 1,
                    PaymentStatus = "Paid",
                    Amount = 100.0,
                    Description = "Payment for invoice 1"
                }
            };

            _mockInvoiceRepo.Setup(r => r.SavePaymentInvoiceAsync(request))
                .ThrowsAsync(new Exception("Database connection error"));

            var result = await _controller.SavePaymentInvoiceAsync(request);

            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task GetPaymentInvoiceDetailByDateAsync_ShouldReturnOk_WhenRecordsFound()
        {
            var fromDate = DateTime.Today.AddDays(-7);
            var toDate = DateTime.Today;
            var mockData = new List<PaymentInvoiceResponse>
            {
                new()
                {
                    DbCode = "TEST_DB",
                    InvoiceId = 1,
                    DeliveryName = "Delivery A",
                    PaymentId = 10,
                    DividedId = 20,
                    CustomerCode = "CUST01",
                    CustomerName = "Customer A",
                    InvoiceCode = "INV01",
                    Amount = 1.0,
                    InvoiceValue = 100.0,
                    HalfPaid = 0,
                    FullPaid = 100.0,
                    Total = 0
                }
            };

            _mockPaymentInvoiceRepo.Setup(r => r.GetPaymentInvoiceDetailByDateAsync("TEST_DB", fromDate, toDate))
                .ReturnsAsync(mockData);

            var result = await _controller.GetPaymentInvoiceDetailByDateAsync(fromDate, toDate);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Success);
            Assert.NotNull(result.Result);
            Assert.Single(result.Result);
        }

        [Fact]
        public async Task GetPaymentInvoiceDetailByDateAsync_ShouldReturnBadRequest_WhenNoRecords()
        {
            var fromDate = DateTime.Today.AddDays(-7);
            var toDate = DateTime.Today;

            _mockPaymentInvoiceRepo.Setup(r => r.GetPaymentInvoiceDetailByDateAsync("TEST_DB", fromDate, toDate))
                .ReturnsAsync(new List<PaymentInvoiceResponse>());

            var result = await _controller.GetPaymentInvoiceDetailByDateAsync(fromDate, toDate);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.NotNull(result.Result);
            Assert.Empty(result.Result);
        }

        [Fact]
        public async Task DeletePaymentInvoiceAsync_ShouldReturnOk_WhenSuccessful()
        {
            _mockPaymentInvoiceRepo.Setup(r => r.DeletePaymentInvoiceAsync(1, 2))
                .ReturnsAsync(1);

            var result = await _controller.DeletePaymentInvoiceAsync(1, 2);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Success);
            Assert.True(result.Result);
        }

        [Fact]
        public async Task DeletePaymentInvoiceAsync_ShouldReturnBadRequest_WhenUnsuccessful()
        {
            _mockPaymentInvoiceRepo.Setup(r => r.DeletePaymentInvoiceAsync(1, 2))
                .ReturnsAsync(0);

            var result = await _controller.DeletePaymentInvoiceAsync(1, 2);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.False(result.Success);
            Assert.False(result.Result);
        }
    }
}
