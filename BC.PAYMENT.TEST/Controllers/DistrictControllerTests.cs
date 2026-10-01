using BC.PAYMENT.API.Controllers.Setting.Preset;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Contracts.Setting.Preset;
using BC.PAYMENT.CORE.Entities.Setting.Preset;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using BC.PAYMENT.APPLICATION.Interfaces.Setting.Preset;
using Xunit;

namespace BC.PAYMENT.TEST.Controllers.Setting.Preset;

public class DistrictControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IDistrictRepository> _mockDistrictRepo;
    private readonly DistrictController _controller;

    public DistrictControllerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockDistrictRepo = new Mock<IDistrictRepository>();
        _mockUnitOfWork.Setup(u => u.Districts).Returns(_mockDistrictRepo.Object);

        _controller = new DistrictController(_mockUnitOfWork.Object);

        var claims = new List<Claim>
        {
            new("UserId", "1"),
            new("Username", "testuser"),
            new("DbCode", "DB01")
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
    public async Task GetDistrict_ReturnsOk_WhenDataExists()
    {
        var mockData = new List<DistrictResponseDTO> { new() };
        _mockDistrictRepo.Setup(r => r.GetAsync("DB01")).ReturnsAsync(mockData);

        var response = await _controller.GetDistrict();

        Assert.NotNull(response);
        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetDistrictByDistrictAsync_ReturnsOk_WhenDataExists()
    {
        var mockData = new List<DistrictResponseDTO> { new() };
        _mockDistrictRepo.Setup(r => r.GetDistrictsByDistrictAsync("Dist1")).ReturnsAsync(mockData);

        var response = await _controller.GetDistrictByDistrictAsync("Dist1");

        Assert.NotNull(response);
        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetDistrictByProvinceAsync_ReturnsOk_WhenDataExists()
    {
        var mockData = new List<DistrictResponseDTO> { new() };
        _mockDistrictRepo.Setup(r => r.GetDistrictsByProvinceAsync("Prov1")).ReturnsAsync(mockData);

        var response = await _controller.GetDistrictByProvinceAsync("Prov1");

        Assert.NotNull(response);
        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteDistrictAsync_ReturnsOk_OnSuccess()
    {
        _mockDistrictRepo.Setup(r => r.DeleteAsync("D01")).ReturnsAsync(1);

        var response = await _controller.DeleteDistrictAsync("D01");

        Assert.NotNull(response);
        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDistrictAsync_ReturnsOk_OnSuccess()
    {
        var request = new DistrictUpdateDto();
        _mockDistrictRepo.Setup(r => r.UpdateAsync(It.IsAny<DistrictModel>())).ReturnsAsync(1);

        var response = await _controller.UpdateDistrictAsync(request);

        Assert.NotNull(response);
        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AddDistrictAsync_ReturnsOk_OnSuccess()
    {
        var request = new DistrictDto();
        _mockDistrictRepo.Setup(r => r.AddNewAsync(It.IsAny<DistrictModel>())).ReturnsAsync(1);

        var response = await _controller.AddDistrictAsync(request);

        Assert.NotNull(response);
        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
    }
}