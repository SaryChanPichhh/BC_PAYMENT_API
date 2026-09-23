using BC.PAYMENT.API.Controllers.General;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Contracts.Request.General;
using BC.PAYMENT.CORE.Entities.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace BC.PAYMENT.TEST.Controllers
{
    public class InvoiceClosingEntryControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IInvoiceClosingEntryRepository> _mockRepo;
        private readonly InvoiceClosingEntryController _controller;

        public InvoiceClosingEntryControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IInvoiceClosingEntryRepository>();

            _mockUnitOfWork.Setup(u => u.InvoiceClosingEntry).Returns(_mockRepo.Object);

            _controller = new InvoiceClosingEntryController(_mockUnitOfWork.Object);

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
        public async Task CheckIsEntriesIsAlreadyOpenAsync_ShouldReturnSuccess()
        {
            _mockRepo.Setup(r => r.CheckIsEntriesIsAlreadyOpenAsync("TEST_DB")).ReturnsAsync(true);

            var result = await _controller.CheckIsEntriesIsAlreadyOpenAsync();

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Result);
        }

        [Fact]
        public async Task CreateClosingEntryAsync_ShouldReturnSuccess()
        {
            var request = new InvoiceClosingEntriesRequest
            {
                Description = "Closing Test"
            };
            _mockRepo.Setup(r => r.CreateClosingEntryAsync(It.IsAny<InvoiceClosingEntriesModel>())).ReturnsAsync(1);

            var result = await _controller.CreateClosingEntryAsync(request);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(1, result.Result);
        }

        [Fact]
        public async Task GenerateOpeningEntryCodeAsync_ShouldReturnCode()
        {
            _mockRepo.Setup(r => r.GenerateOpeningEntryCodeAsync("TEST_DB")).ReturnsAsync("OPEN01");

            var result = await _controller.GenerateOpeningEntryCodeAsync();

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("OPEN01", result.Result);
        }

        [Fact]
        public async Task GetOpeningEntryCodeByDbCodeAsync_ShouldReturnCode()
        {
            _mockRepo.Setup(r => r.GetOpeningEntryCodeByDbCodeAsync("TEST_DB")).ReturnsAsync("ENTRY_01");

            var result = await _controller.GetOpeningEntryCodeByDbCodeAsync();

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("ENTRY_01", result.Result);
        }
    }
}
