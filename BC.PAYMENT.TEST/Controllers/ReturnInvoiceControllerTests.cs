using BC.PAYMENT.API.Controllers.Invoice;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.APPLICATION.Interfaces.Invoice;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
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
    public class ReturnInvoiceControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IReturnInvoiceRepository> _mockRepo;
        private readonly ReturnInvoiceController _controller;

        public ReturnInvoiceControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IReturnInvoiceRepository>();

            _mockUnitOfWork.Setup(u => u.ReturnInvoice).Returns(_mockRepo.Object);

            _controller = new ReturnInvoiceController(_mockUnitOfWork.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("DbCode", "TEST_DB"),
                new Claim("Username", "TEST_USER"),
                new Claim("InvoiceEntryCode", "ENTRY_01"),
                new Claim("CurrentDate", DateTime.Today.ToString("MM/dd/yyyy"))
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetReturnInvoiceAsync_ShouldReturnOk_WhenInvoicesExist()
        {
            var mockData = new List<ReturnInvoiceResponse>
            {
                new() { InvoiceId = 1, TransactionCode = "INV01", CustomerCode = "CUS01", InvoiceValue = 100 }
            };
            _mockRepo.Setup(r => r.GetReturnInvoiceAsync("TEST_DB")).ReturnsAsync(mockData);

            var result = await _controller.GetReturnInvoiceAsync();

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(result.Result);
            Assert.Single(result.Result);
            Assert.Equal("INV01", result.Result[0].TransactionCode);
        }

        [Fact]
        public async Task GetReturnInvoiceAsync_ShouldReturnBadRequest_WhenEmpty()
        {
            _mockRepo.Setup(r => r.GetReturnInvoiceAsync("TEST_DB")).ReturnsAsync(new List<ReturnInvoiceResponse>());

            var result = await _controller.GetReturnInvoiceAsync();

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.NotNull(result.Result);
            Assert.Empty(result.Result);
        }

        [Fact]
        public async Task GetReturnInvoiceByDateAsync_ShouldReturnOk_WhenInvoicesExist()
        {
            var mockData = new List<ReturnInvoiceResponse>
            {
                new() { InvoiceId = 2, TransactionCode = "INV02", CustomerCode = "CUS02", InvoiceValue = 250 }
            };
            var fromDate = DateTime.Today.AddDays(-7);
            var toDate = DateTime.Today;
            _mockRepo.Setup(r => r.GetPcReturnInvoiceByDateAsync("TEST_DB", fromDate, toDate)).ReturnsAsync(mockData);

            var result = await _controller.GetPcReturnInvoiceByDateAsync(fromDate, toDate);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(result.Result);
            Assert.Single(result.Result);
            Assert.Equal("INV02", result.Result[0].TransactionCode);
        }

        [Fact]
        public async Task GetReturnInvoiceByDateAsync_ShouldReturnBadRequest_WhenEmpty()
        {
            var fromDate = DateTime.Today.AddDays(-7);
            var toDate = DateTime.Today;
            _mockRepo.Setup(r => r.GetPcReturnInvoiceByDateAsync("TEST_DB", fromDate, toDate)).ReturnsAsync(new List<ReturnInvoiceResponse>());

            var result = await _controller.GetPcReturnInvoiceByDateAsync(fromDate, toDate);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.NotNull(result.Result);
            Assert.Empty(result.Result);
        }

        [Fact]
        public async Task SaveReturnInvoiceAsync_ShouldReturnOk_WhenSavedSuccessfully()
        {
            var request = new List<ReturnInvoiceRequest>
            {
                new() { TransactionCode = "INV01", CustomerCode = "CUS01", CustomerName = "John", InvoiceValue = 100 }
            };
            _mockRepo.Setup(r => r.SaveReturnInvoiceAsync(request, "TEST_DB", "TEST_USER", "ENTRY_01")).ReturnsAsync(1);

            var result = await _controller.SaveReturnInvoiceAsync(request);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(1, result.Result);
        }

        [Fact]
        public async Task SaveReturnInvoiceAsync_ShouldReturnNotFound_WhenRowsAffectedZero()
        {
            var request = new List<ReturnInvoiceRequest>
            {
                new() { TransactionCode = "INV01", CustomerCode = "CUS01", CustomerName = "John", InvoiceValue = 100 }
            };
            _mockRepo.Setup(r => r.SaveReturnInvoiceAsync(request, "TEST_DB", "TEST_USER", "ENTRY_01")).ReturnsAsync(0);

            var result = await _controller.SaveReturnInvoiceAsync(request);

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetPcReturnInvoiceByDateAsync_ShouldReturnOk_WhenInvoicesExist()
        {
            var mockData = new List<ReturnInvoiceResponse>
            {
                new() { InvoiceId = 1, TransactionCode = "INV01", CustomerCode = "CUS01", InvoiceValue = 100 }
            };
            var fromDate = DateTime.Today.AddDays(-7);
            var toDate = DateTime.Today;
            _mockRepo.Setup(r => r.GetPcReturnInvoiceByDateAsync("TEST_DB", fromDate, toDate)).ReturnsAsync(mockData);

            var result = await _controller.GetPcReturnInvoiceByDateAsync(fromDate, toDate);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Success);
            Assert.NotNull(result.Result);
            Assert.Single(result.Result);
            Assert.Equal("INV01", result.Result[0].TransactionCode);
        }

        [Fact]
        public async Task GetPcReturnInvoiceByDateAsync_ShouldReturnBadRequest_WhenEmpty()
        {
            var fromDate = DateTime.Today.AddDays(-7);
            var toDate = DateTime.Today;
            _mockRepo.Setup(r => r.GetPcReturnInvoiceByDateAsync("TEST_DB", fromDate, toDate)).ReturnsAsync(new List<ReturnInvoiceResponse>());

            var result = await _controller.GetPcReturnInvoiceByDateAsync(fromDate, toDate);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.NotNull(result.Result);
            Assert.Empty(result.Result);
        }

        [Fact]
        public async Task GetPcReturnInvoiceByPeriodAsync_ShouldReturnOk_WhenInvoicesExist()
        {
            var mockData = new List<ReturnInvoiceResponse>
            {
                new() { InvoiceId = 2, TransactionCode = "INV02", CustomerCode = "CUS02", InvoiceValue = 200 }
            };
            _mockRepo.Setup(r => r.GetPcReturnInvoiceByPeriodAsync("TEST_DB", "05", "2026")).ReturnsAsync(mockData);

            var result = await _controller.GetPcReturnInvoiceByPeriodAsync("05", "2026");

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Success);
            Assert.NotNull(result.Result);
            Assert.Single(result.Result);
            Assert.Equal("INV02", result.Result[0].TransactionCode);
        }

        [Fact]
        public async Task GetPcReturnInvoiceByPeriodAsync_ShouldReturnBadRequest_WhenEmpty()
        {
            _mockRepo.Setup(r => r.GetPcReturnInvoiceByPeriodAsync("TEST_DB", "05", "2026")).ReturnsAsync(new List<ReturnInvoiceResponse>());

            var result = await _controller.GetPcReturnInvoiceByPeriodAsync("05", "2026");

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.NotNull(result.Result);
            Assert.Empty(result.Result);
        }

        [Fact]
        public async Task DeletePcReturnInvoiceAsync_ShouldReturnOk_WhenDeleteSucceeds()
        {
            _mockRepo.Setup(r => r.DeletePcReturnInvoiceAsync(10)).ReturnsAsync(1);

            var result = await _controller.DeletePcReturnInvoiceAsync(10);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Success);
            Assert.Equal(1, result.Result);
        }

        [Fact]
        public async Task DeletePcReturnInvoiceAsync_ShouldReturnBadRequest_WhenDeleteFails()
        {
            _mockRepo.Setup(r => r.DeletePcReturnInvoiceAsync(10)).ReturnsAsync(0);

            var result = await _controller.DeletePcReturnInvoiceAsync(10);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(0, result.Result);
        }
    }
}
