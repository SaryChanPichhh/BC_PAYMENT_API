using BC.PAYMENT.API.Controllers;
using BC.PAYMENT.CORE.Contracts.Request.Area;
using BC.PAYMENT.CORE.Contracts.Response.Area;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace BC.PAYMENT.TEST.Controllers;

public class AreaControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IAreaRepository> _mockRepo;
    private readonly AreaController _controller;

    public AreaControllerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockRepo = new Mock<IAreaRepository>();

        _mockUnitOfWork.Setup(u => u.Areas).Returns(_mockRepo.Object);

        _controller = new AreaController(_mockUnitOfWork.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new("DbCode", "TEST_DB"),
            new("Username", "TEST_USER")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GetArea_ShouldReturnOk_WhenAreasExist()
    {
        var mockAreas = new List<AreaResponse>
        {
            new() { AreaId = "A01", AreaName = "North" }
        };
        _mockRepo.Setup(r => r.GetArea("TEST_DB")).ReturnsAsync(mockAreas);

        var result = await _controller.GetArea();

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.Single(result.Result);
        Assert.Equal("North", result.Result[0].AreaName);
    }

    [Fact]
    public async Task CreateArea_ShouldReturnCreated_WhenSuccessful()
    {
        var request = new CreateAreaRequest { AreaName = "South" };
        _mockRepo.Setup(r => r.CreateAreaAsync(It.IsAny<Area>())).ReturnsAsync(true);

        var result = await _controller.CreateArea(request);

        Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
        Assert.True(result.Result);
    }

    [Fact]
    public async Task UpdateArea_ShouldReturnOk_WhenSuccessful()
    {
        var request = new UpdateAreaRequest { AreaName = "Updated" };
        _mockRepo.Setup(r => r.UpdateAreaAsync(It.IsAny<Area>())).ReturnsAsync(true);

        var result = await _controller.UpdateArea("A01", request);

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.True(result.Result);
    }

    [Fact]
    public async Task DeleteArea_ShouldReturnOk_WhenSuccessful()
    {
        _mockRepo.Setup(r => r.DeleteAreaAsync("A01", "TEST_DB")).ReturnsAsync(true);

        var result = await _controller.DeleteArea("A01");

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.True(result.Result);
    }

    [Fact]
    public async Task GenerateAreaId_ShouldReturnOk_WhenSuccessful()
    {
        _mockRepo.Setup(r => r.GenerateAreaIdAsync()).ReturnsAsync("AREA-123");

        var result = await _controller.GenerateAreaId();

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("AREA-123", result.Result);
    }
}