using System.Security.Claims;
using BC.PAYMENT.API.Controllers.Setting.Preset;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.APPLICATION.Interfaces.Setting.Preset;
using BC.PAYMENT.CORE.Contracts.Request.Province;
using BC.PAYMENT.CORE.Contracts.Response.Province;
using BC.PAYMENT.CORE.Entities.Setting.Preset;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BC.PAYMENT.TEST.Controllers
{
    public class ProvinceControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IProvinceRepository> _mockProvinceRepo;
        private readonly Mock<IDistrictRepository> _mockDistrictRepo;
        private readonly ProvinceController _controller;

        public ProvinceControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockProvinceRepo = new Mock<IProvinceRepository>();
            _mockDistrictRepo = new Mock<IDistrictRepository>();
            
            _mockUnitOfWork.Setup(u => u.Provinces).Returns(_mockProvinceRepo.Object);
            _mockUnitOfWork.Setup(u => u.Districts).Returns(_mockDistrictRepo.Object);

            _controller = new ProvinceController(_mockUnitOfWork.Object);

            var claims = new List<Claim>
            {
                new Claim("UserId", "1"),
                new Claim("Username", "testuser"),
                new Claim("DbCode", "DB01")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var userPrincipal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext { User = userPrincipal };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task GetProvinceAsync_ReturnsOk_WhenDataExists()
        {
            // Arrange
            var mockData = new List<ProvinceResponse> { new ProvinceResponse() };
            _mockProvinceRepo.Setup(r => r.GetAllProvinces()).ReturnsAsync(mockData);

            // Act
            var response = await _controller.GetProvinceAsync();

            // Assert
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteProvinceAsync_ReturnsOk_OnSuccess()
        {
            // Arrange
            _mockDistrictRepo.Setup(r => r.DeleteAsync("P01")).ReturnsAsync(1); // The controller uses Districts.DeleteAsync

            // Act
            var response = await _controller.DeleteProvinceAsync("P01");

            // Assert
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, response.Result);
        }

        [Fact]
        public async Task UpdateProvinceAsync_ReturnsOk_OnSuccess()
        {
            // Arrange
            var request = new ProvinceUpdateRequest { ProvinceId = 1, Province = "Test" };
            _mockProvinceRepo.Setup(r => r.UpdateAsync(It.IsAny<ProvinceModel>())).ReturnsAsync(1);

            // Act
            var response = await _controller.UpdateProvinceAsync(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, response.Result);
        }

        [Fact]
        public async Task AddDistrictAsync_ReturnsOk_OnSuccess() // Name is AddDistrictAsync in the controller
        {
            // Arrange
            var request = new ProvinceCreateRequest { Province = "Test" };
            _mockProvinceRepo.Setup(r => r.AddNewAsync(It.IsAny<ProvinceModel>())).ReturnsAsync(1);

            // Act
            var response = await _controller.AddDistrictAsync(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, response.Result);
        }
    }
}
