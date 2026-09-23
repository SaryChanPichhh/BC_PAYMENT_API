using BC.PAYMENT.API.Controllers.Invoice;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.APPLICATION.Interfaces.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace BC.PAYMENT.TEST.Controllers
{
    public class InvoicesControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IInvoiceRepository> _mockInvoiceRepo;
        private readonly Mock<IOptions<AppSettings>> _mockOptions;
        private readonly InvoiceController _controller;

        public InvoicesControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockInvoiceRepo = new Mock<IInvoiceRepository>();
            _mockOptions = new Mock<IOptions<AppSettings>>();

            var appSettings = new AppSettings
            {
                Secret = "test-secret",
                PYS_Key = "test-pys-key",
                HR_Key = "test-hr-key",
                ACC_Key = "test-acc-key",
                Issuer = "test-issuer",
                Audience = "test-audience",
                Subject = "test-subject",
                AES_KEY = "test-aes-key",
                AES_IV = "test-aes-iv",
                ItemImageBaseUrl = "http://localhost/images"
            };
            _mockOptions.Setup(o => o.Value).Returns(appSettings);

            _mockUnitOfWork.Setup(u => u.Invoices).Returns(_mockInvoiceRepo.Object);

            _controller = new InvoiceController(_mockUnitOfWork.Object);

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
        public async Task GetInvoiceTransactionByDeliveryAndDateAsync_ShouldReturnOk_WhenFound()
        {
            var mockData = new List<DividedInvoiceTransactionResponse>
            {
                new()
                {
                    InvoiceId = 1,
                    DividedId = 10,
                    InvoiceCode = "INV01",
                    CustomerCode = "CUST01",
                    CustomerName = "Customer A",
                    InvoiceValue = 200,
                    IsReturn = 0,
                    PaidValue = 100,
                    Amount = 100,
                    IsPaid = true
                }
            };
            var date = DateTime.Today;
            _mockInvoiceRepo.Setup(r => r.GetInvoiceTransactionByDeliveryAndDateAsync("TEST_DB", "DEL01", date))
                .ReturnsAsync(mockData);

            var result = await _controller.GetInvoiceTransactionByDeliveryAndDateAsync("DEL01", date);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Success);
            Assert.NotNull(result.Result);
            Assert.Single(result.Result);
            Assert.Equal("Invoice transaction fetched successfully", result.Message);
        }

        [Fact]
        public async Task GetInvoiceTransactionByDeliveryAndDateAsync_ShouldReturnBadRequest_WhenEmpty()
        {
            var date = DateTime.Today;
            _mockInvoiceRepo.Setup(r => r.GetInvoiceTransactionByDeliveryAndDateAsync("TEST_DB", "DEL01", date))
                .ReturnsAsync(new List<DividedInvoiceTransactionResponse>());

            var result = await _controller.GetInvoiceTransactionByDeliveryAndDateAsync("DEL01", date);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.NotNull(result.Result);
            Assert.Empty(result.Result);
            Assert.Equal("Invoice transaction fetched unsuccessfully", result.Message);
        }

        [Fact]
        public async Task GetInvoiceTransactionByDeliveryAndDateAsync_ShouldReturnInternalServerError_WhenExceptionThrown()
        {
            var date = DateTime.Today;
            _mockInvoiceRepo.Setup(r => r.GetInvoiceTransactionByDeliveryAndDateAsync("TEST_DB", "DEL01", date))
                .ThrowsAsync(new Exception("Database error"));

            var result = await _controller.GetInvoiceTransactionByDeliveryAndDateAsync("DEL01", date);

            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task PostInvoiceAsync_ShouldReturnOk_WhenSuccessful()
        {
            _mockInvoiceRepo.Setup(r => r.PostPrintInvoiceAsync("INV001", BC.PAYMENT.CORE.Enums.RequestType.Invoice, "TEST_DB", It.IsAny<string>(), "TEST_USER"))
                .ReturnsAsync(true);

            var result = await _controller.PostInvoiceAsync("INV001", BC.PAYMENT.CORE.Enums.RequestType.Invoice);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Success);
            Assert.True(result.Result);
            Assert.Equal("Invoice posted successfully", result.Message);
        }

        [Fact]
        public async Task PostInvoiceAsync_ShouldReturnBadRequest_WhenFailed()
        {
            _mockInvoiceRepo.Setup(r => r.PostPrintInvoiceAsync("INV001", BC.PAYMENT.CORE.Enums.RequestType.Invoice, "TEST_DB", It.IsAny<string>(), "TEST_USER"))
                .ReturnsAsync(false);

            var result = await _controller.PostInvoiceAsync("INV001", BC.PAYMENT.CORE.Enums.RequestType.Invoice);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.False(result.Success);
            Assert.False(result.Result);
            Assert.Equal("Invoice posted unsuccessfully", result.Message);
        }
    }
}
