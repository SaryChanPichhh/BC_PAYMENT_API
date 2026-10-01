using BC.PAYMENT.API.Controllers.Invoice;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.APPLICATION.Interfaces.Invoice;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using BC.PAYMENT.CORE.Entities.Invoice;
using BC.PAYMENT.CORE.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace BC.PAYMENT.TEST.Controllers;

public class NewInvoiceControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<INewInvoiceRepository> _mockRepo;
    private readonly NewInvoiceController _controller;

    public NewInvoiceControllerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockRepo = new Mock<INewInvoiceRepository>();

        _mockUnitOfWork.Setup(u => u.NewInvoice).Returns(_mockRepo.Object);

        _controller = new NewInvoiceController(_mockUnitOfWork.Object);

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
    public async Task GetInvoicesAsync_ShouldReturnOk_WhenInvoicesExist()
    {
        var mockData = new List<NewInvoiceResponse>
        {
            new() { InvoiceCode = "NEW01", CustomerCode = "C01", InvoiceAmount = 100 }
        };
        _mockRepo.Setup(r => r.GetInvoices(InvoiceStatus.NewInvoice, "TEST_DB", It.IsAny<DateTime>()))
            .ReturnsAsync(mockData);

        var result = await _controller.GetInvoicesAsync();

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Single(result.Result);
        Assert.Equal("NEW01", result.Result[0].InvoiceCode);
    }

    [Fact]
    public async Task GetInvoicesAsync_ShouldReturnBadRequest_WhenEmpty()
    {
        _mockRepo.Setup(r => r.GetInvoices(InvoiceStatus.NewInvoice, "TEST_DB", It.IsAny<DateTime>()))
            .ReturnsAsync(new List<NewInvoiceResponse>());

        var result = await _controller.GetInvoicesAsync();

        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Empty(result.Result);
    }

    [Fact]
    public async Task GetInvoicesByInvoiceCodeAsync_ShouldReturnOk_WhenSuccessful()
    {
        var fromDate = DateTime.Today.AddDays(-5);
        var toDate = DateTime.Today;
        var mockData = new List<NewInvoiceResponse>
        {
            new() { InvoiceCode = "NEW01", CustomerCode = "C01", InvoiceAmount = 100 }
        };
        _mockRepo.Setup(r => r.GetInvoicesByInvoiceCode("INV01", "INV02", fromDate, toDate, "TEST_DB"))
            .ReturnsAsync(mockData);
        _mockRepo.Setup(r => r.SaveInvoices(It.IsAny<List<NewInvoiceModel>>()))
            .ReturnsAsync(1);

        var result = await _controller.GetInvoicesByInvoiceCodeAsync("INV01", "INV02", fromDate, toDate);

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Single(result.Result);
    }

    [Fact]
    public async Task AddNewInvoiceAsync_ShouldReturnOk_WhenSuccessful()
    {
        var request = new List<NewInvoiceRequest>
        {
            new() { InvoiceCode = "NEW01", CustomerCode = "C01", CustomerName = "John", InvoiceAmount = 100 }
        };
        _mockRepo.Setup(r => r.SaveInvoices(It.IsAny<List<NewInvoiceModel>>())).ReturnsAsync(1);

        var result = await _controller.AddNewInvoiceAsync(request);

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(1, result.Result);
    }

    [Fact]
    public async Task AddNewInvoiceAsync_ShouldReturnBadRequest_WhenFailed()
    {
        var request = new List<NewInvoiceRequest>
        {
            new() { InvoiceCode = "NEW01", CustomerCode = "C01", CustomerName = "John", InvoiceAmount = 100 }
        };
        _mockRepo.Setup(r => r.SaveInvoices(It.IsAny<List<NewInvoiceModel>>())).ReturnsAsync(0);

        var result = await _controller.AddNewInvoiceAsync(request);

        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task DeleteInvoiceAsync_ShouldReturnOk_WhenSuccessful()
    {
        _mockRepo.Setup(r => r.DeleteInvoiceAsync(1)).ReturnsAsync(1);

        var result = await _controller.DeleteInvoiceAsync(1);

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.True(result.Result);
    }

    [Fact]
    public async Task DeleteInvoiceAsync_ShouldReturnBadRequest_WhenFailed()
    {
        _mockRepo.Setup(r => r.DeleteInvoiceAsync(1)).ReturnsAsync(0);

        var result = await _controller.DeleteInvoiceAsync(1);

        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        Assert.False(result.Result);
    }
}