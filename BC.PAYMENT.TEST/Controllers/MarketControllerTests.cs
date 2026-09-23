using BC.PAYMENT.API.Controllers;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Contracts.Request.Market;
using BC.PAYMENT.CORE.Contracts.Response.Market;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Entities.Setting.Preset;
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
    public class MarketControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMarketRepository> _mockMarketRepo;
        private readonly MarketController _controller;
        private readonly ClaimsPrincipal _userPrincipal;

        public MarketControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMarketRepo = new Mock<IMarketRepository>();
            _mockUnitOfWork.Setup(u => u.Markets).Returns(_mockMarketRepo.Object);
            
            _controller = new MarketController(_mockUnitOfWork.Object);

            // Set up mocked User Claims for DecodeJwt
            var claims = new List<Claim>
            {
                new Claim("UserId", "1"),
                new Claim("Username", "testuser"),
                new Claim("DbCode", "DB01"),
                new Claim("AppCode", "APP01"),
                new Claim("CompanyCode", "COMP01"),
                new Claim("CurrentDate", DateTime.Now.ToString("MM/dd/yyyy")),
                new Claim("InvoiceEntryCode", "INV01"),
                new Claim("Period", "202310")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            _userPrincipal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext { User = _userPrincipal };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task GetMarket_ReturnsOk_WhenMarketsExist()
        {
            // Arrange
            var mockMarkets = new List<MarketResponse>
            {
                new MarketResponse { MarketId = "1", MarketName = "Test Market", Image = "" }
            };
            _mockMarketRepo.Setup(r => r.GetMarket("DB01")).ReturnsAsync(mockMarkets);

            // Act
            var response = await _controller.GetMarket();

            // Assert
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, response?.Result?.Count);
        }

        [Fact]
        public async Task GetMarket_ReturnsBadRequest_WhenNoMarkets()
        {
            // Arrange
            _mockMarketRepo.Setup(r => r.GetMarket("DB01")).ReturnsAsync(new List<MarketResponse>());

            // Act
            var response = await _controller.GetMarket();

            // Assert
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("No new markets", response.Message);
        }

        [Fact]
        public async Task GetMarketImage_ReturnsFile_WhenImageExists()
        {
            // Arrange
            const string marketId = "M01";
            var encryptedId = Uri.EscapeDataString(EncryptionHelper.EncryptAES(marketId));
            var imageBytes = new byte[] { 1, 2, 3 };
            _mockMarketRepo.Setup(r => r.GetMarketImageAsync(It.IsAny<string>())).ReturnsAsync(imageBytes);

            // Act
            var result = await _controller.GetMarketImage(encryptedId) as FileContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("image/jpeg", result?.ContentType);
            Assert.Equal(imageBytes.Length, result?.FileContents.Length);
        }

        [Fact]
        public async Task GetMarketImage_ReturnsNotFound_WhenImageEmpty()
        {
            // Arrange
            const string marketId = "M01";
            var encryptedId = Uri.EscapeDataString(EncryptionHelper.EncryptAES(marketId));
            _mockMarketRepo.Setup(r => r.GetMarketImageAsync(It.IsAny<string>())).ReturnsAsync(Array.Empty<byte>());

            // Act
            var result = await _controller.GetMarketImage(encryptedId) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Image not found", result?.Value);
        }

        [Fact]
        public async Task LoadMarketBySaleTypes_ReturnsOk()
        {
            // Arrange
            var saleTypes = new List<string> { "TypeA" };
            var mockMarkets = new List<Market> { new Market() };
            _mockMarketRepo.Setup(r => r.LoadMarketBySaleTypesAsync("DB01", saleTypes, 1, 10)).ReturnsAsync(mockMarkets);

            // Act
            var response = await _controller.LoadMarketBySaleTypes(saleTypes, 1, 10);

            // Assert
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, response?.Result?.Count);
        }

        [Fact]
        public async Task GetMarketByDbCode_ReturnsOk()
        {
            // Arrange
            var mockMarkets = new List<MarketResponse> { new MarketResponse() };
            _mockMarketRepo.Setup(r => r.GetMarketByDbCodeAsync("DB01")).ReturnsAsync(mockMarkets);

            // Act
            var response = await _controller.GetMarketByDbCode();

            // Assert
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, response?.Result?.Count);
        }

        [Fact]
        public async Task GetMarketByMarketIdAsync_ReturnsOk()
        {
            // Arrange
            var mockMarket = new MarketResponse { MarketId = "M01" };
            _mockMarketRepo.Setup(r => r.GetMarketByMarketIdAsync("DB01", "M01")).ReturnsAsync(mockMarket);

            // Act
            var response = await _controller.GetMarketByMarketIdAsync("M01");

            // Assert
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("M01", response?.Result?.MarketId);
        }

        [Fact]
        public async Task AddNewMarket_ReturnsCreated_OnSuccess()
        {
            // Arrange
            var request = new MarketCreateRequest { MarketId = "M01" };
            _mockMarketRepo.Setup(r => r.AddNewMarketAsync(It.IsAny<MarketModel>())).ReturnsAsync(1);

            // Act
            var response = await _controller.AddNewMarket(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(1, response.Result);
        }

        [Fact]
        public async Task UpdateMarket_ReturnsOk_OnSuccess()
        {
            // Arrange
            var model = new MarketUpdateRequest() { MarketId = "M01" };
            _mockMarketRepo.Setup(r => r.UpdateMarketAsync(It.IsAny<MarketModel>())).ReturnsAsync(1);

            // Act
            var response = await _controller.UpdateMarket(model);

            // Assert
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, response.Result);
        }

        [Fact]
        public async Task DeleteMarket_ReturnsOk_OnSuccess()
        {
            // Arrange
            _mockMarketRepo.Setup(r => r.DeleteMarketAsync("M01")).ReturnsAsync(1);

            // Act
            var response = await _controller.DeleteMarket("M01");

            // Assert
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, response.Result);
        }

        [Fact]
        public async Task GenerateMarketIdAsync_ReturnsOk_OnSuccess()
        {
            // Arrange
            _mockMarketRepo.Setup(r => r.GenerateMarketIdAsync()).ReturnsAsync("MK-001");

            // Act
            var response = await _controller.GenerateMarketIdAsync();

            // Assert
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("MK-001", response.Result);
        }

        [Fact]
        public async Task AddNewMarket_ReturnsExceptionError_OnException()
        {
            // Arrange
            var request = new MarketCreateRequest { MarketId = "M01" };
            _mockMarketRepo.Setup(r => r.AddNewMarketAsync(It.IsAny<MarketModel>())).ThrowsAsync(new Exception("Database error"));
            // Act
            var response = await _controller.AddNewMarket(request);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Success);
            // Since GlobalExceptionHandler creates an error response, we assume it has Success = false and the message contains the exception.
            Assert.Equal("Database error", response.Message);
        }
    }
}
