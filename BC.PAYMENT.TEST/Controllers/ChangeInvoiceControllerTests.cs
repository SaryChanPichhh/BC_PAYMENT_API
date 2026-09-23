using BC.PAYMENT.API.Controllers.Invoice;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.APPLICATION.Interfaces.Invoice;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;
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
    public class ChangeInvoiceControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IChangeInvoiceRepository> _mockRepo;
        private readonly ChangeInvoiceController _controller;

        public ChangeInvoiceControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IChangeInvoiceRepository>();

            _mockUnitOfWork.Setup(u => u.ChangeInvoice).Returns(_mockRepo.Object);

            _controller = new ChangeInvoiceController(_mockUnitOfWork.Object);

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
        public async Task GetChangeInvoicesAsync_ShouldReturnOk_WhenInvoicesExist()
        {
            var mockData = new List<ChangeInvoiceResponse>
            {
                new() { InvoiceId = 1, InvoiceCode = "INV01", CustomerCode = "C01", InvoiceAmount = 100 }
            };
            _mockRepo.Setup(r => r.GetChangeInvoicesAsync("TEST_DB", It.IsAny<DateTime>())).ReturnsAsync(mockData);

            var result = await _controller.GetChangeInvoicesAsync();

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Single(result.Result);
            Assert.Equal("INV01", result.Result[0].InvoiceCode);
        }

        [Fact]
        public async Task GetChangeInvoicesAsync_ShouldReturnBadRequest_WhenEmpty()
        {
            _mockRepo.Setup(r => r.GetChangeInvoicesAsync("TEST_DB", It.IsAny<DateTime>())).ReturnsAsync(new List<ChangeInvoiceResponse>());

            var result = await _controller.GetChangeInvoicesAsync();

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Empty(result.Result);
        }

        [Fact]
        public async Task CheckExistInvoiceAsync_ShouldReturnOk()
        {
            _mockRepo.Setup(r => r.CheckExistInvoiceAsync("INV01", "TEST_DB")).ReturnsAsync(true);

            var result = await _controller.CheckExistInvoiceAsync("INV01");

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Result);
        }

        [Fact]
        public async Task GetLocalInvoiceAsync_ShouldReturnOk_WhenInvoicesExist()
        {
            var mockData = new List<ChangeInvoiceResponse>
            {
                new() { InvoiceCode = "LOC01", CustomerCode = "C01", InvoiceAmount = 150 }
            };
            var fromDate = DateTime.Today.AddDays(-5);
            var toDate = DateTime.Today;
            _mockRepo.Setup(r => r.GetLocalInvoiceAsync("TEST_DB", fromDate, toDate)).ReturnsAsync(mockData);

            var result = await _controller.GetLocalInvoiceAsync(fromDate, toDate);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Single(result.Result);
        }

        [Fact]
        public async Task GetLocalInvoiceAsync_ShouldReturnBadRequest_WhenEmpty()
        {
            var fromDate = DateTime.Today.AddDays(-5);
            var toDate = DateTime.Today;
            _mockRepo.Setup(r => r.GetLocalInvoiceAsync("TEST_DB", fromDate, toDate)).ReturnsAsync(new List<ChangeInvoiceResponse>());

            var result = await _controller.GetLocalInvoiceAsync(fromDate, toDate);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Empty(result.Result);
        }

        [Fact]
        public async Task GetOtherBranchesInvoiceAsync_ShouldReturnOk_WhenInvoicesExist()
        {
            var mockData = new List<ChangeInvoiceResponse>
            {
                new() { InvoiceCode = "OTH01", CustomerCode = "C02", InvoiceAmount = 300 }
            };
            var fromDate = DateTime.Today.AddDays(-5);
            var toDate = DateTime.Today;
            _mockRepo.Setup(r => r.GetOtherBranchInvoiceAsync("TEST_DB", fromDate, toDate)).ReturnsAsync(mockData);

            var result = await _controller.GetOtherBranchesInvoiceAsync(fromDate, toDate);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Single(result.Result);
        }

        [Fact]
        public async Task AddChangeInvoiceAsync_ShouldReturnOk_WhenSuccessful()
        {
            var request = new ChangeInvoiceRequest
            {
                TransactionCode = "INV01",
                CustomerCode = "C01",
                CustomerName = "Test",
                InvoiceValue = 100,
                IsExists = false
            };
            _mockRepo.Setup(r => r.AddChangeInvoiceAsync(It.IsAny<ChangeInvoiceModel>())).ReturnsAsync(1);

            var result = await _controller.AddChangeInvoiceAsync(request);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Result);
        }

        [Fact]
        public async Task AddChangeInvoiceAsync_ShouldReturnBadRequest_WhenFailed()
        {
            var request = new ChangeInvoiceRequest
            {
                TransactionCode = "INV01",
                CustomerCode = "C01",
                CustomerName = "Test",
                InvoiceValue = 100,
                IsExists = false
            };
            _mockRepo.Setup(r => r.AddChangeInvoiceAsync(It.IsAny<ChangeInvoiceModel>())).ReturnsAsync(0);

            var result = await _controller.AddChangeInvoiceAsync(request);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.False(result.Result);
        }
    }
}
