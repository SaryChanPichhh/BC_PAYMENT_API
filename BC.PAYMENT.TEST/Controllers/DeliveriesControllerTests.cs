using BC.PAYMENT.API.Controllers;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.API.Models.Deliveries;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace BC.PAYMENT.TEST.Controllers;

public class DeliveriesControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IDeliveryRepository> _mockDeliveryRepo;
    private readonly Mock<IOptions<AppSettings>> _mockAppSettings;
    private readonly DeliveriesController _controller;

    public DeliveriesControllerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockDeliveryRepo = new Mock<IDeliveryRepository>();
        _mockAppSettings = new Mock<IOptions<AppSettings>>();

        _mockUnitOfWork.Setup(u => u.Deliveries).Returns(_mockDeliveryRepo.Object);

        _controller = new DeliveriesController(_mockUnitOfWork.Object, _mockAppSettings.Object);

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
    public async Task CreateDelivery_ShouldReturnCreated_WhenSuccessful()
    {
        // Arrange
        var request = new CreateDeliveryRequest
        {
            DbCode = "TEST_DB",
            DeliveryName = "Test Delivery",
            Status = "ACTIVE"
        };

        _mockDeliveryRepo.Setup(r => r.CreateDelivery(It.IsAny<Delivery>())).ReturnsAsync(true);

        // Act
        var result = await _controller.CreateDelivery(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
        Assert.Equal("Delivery created successfully.", result.Message);
        Assert.NotNull(result.Result);
        Assert.Equal("Test Delivery", result.Result.DeliveryName);
    }

    [Fact]
    public async Task CreateDelivery_ShouldReturnBadRequest_WhenFailed()
    {
        // Arrange
        var request = new CreateDeliveryRequest();
        _mockDeliveryRepo.Setup(r => r.CreateDelivery(It.IsAny<Delivery>())).ReturnsAsync(false);

        // Act
        var result = await _controller.CreateDelivery(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Equal("Failed to create delivery", result.Message);
    }

    [Fact]
    public async Task UpdateDelivery_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var id = "DEL-001";
        var request = new UpdateDeliveryRequest
        {
            DeliveryName = "Updated Delivery",
            Status = "INACTIVE"
        };

        _mockDeliveryRepo.Setup(r => r.UpdateDelivery(It.IsAny<Delivery>())).ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateDelivery(id, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("Delivery updated successfully.", result.Message);
        Assert.NotNull(result.Result);
        Assert.Equal(id, result.Result.DeliveryId);
        Assert.Equal("Updated Delivery", result.Result.DeliveryName);
    }

    [Fact]
    public async Task DeleteDelivery_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var id = "DEL-001";
        _mockDeliveryRepo.Setup(r => r.DeleteDelivery(id)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteDelivery(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.True(result.Result);
    }

    [Fact]
    public async Task DeleteDelivery_ShouldReturnNotFound_WhenFailed()
    {
        // Arrange
        var id = "DEL-001";
        _mockDeliveryRepo.Setup(r => r.DeleteDelivery(id)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteDelivery(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        Assert.False(result.Result);
    }
}