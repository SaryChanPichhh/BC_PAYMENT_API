using BC.PAYMENT.API.Controllers.Invoice;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.APPLICATION.Interfaces.Invoice;
using BC.PAYMENT.CORE.Contracts.Invoice;
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

namespace BC.PAYMENT.TEST.Controllers;

public class DividedInvoiceControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IDividedInvoiceRepository> _mockRepo;
    private readonly Mock<IInvoiceRepository> _mockInvoiceRepo;
    private readonly DividedInvoiceController _controller;

    public DividedInvoiceControllerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockRepo = new Mock<IDividedInvoiceRepository>();
        _mockInvoiceRepo = new Mock<IInvoiceRepository>();

        _mockUnitOfWork.Setup(u => u.DividedInvoice).Returns(_mockRepo.Object);
        _mockUnitOfWork.Setup(u => u.Invoices).Returns(_mockInvoiceRepo.Object);

        _controller = new DividedInvoiceController(_mockUnitOfWork.Object);

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
    public async Task GetDividedInvoiceByDeliverIdAndDate_ShouldReturnOk_WhenFound()
    {
        var mockData = new List<DividedInvoiceResponse>
        {
            new() { Id = "1", DeliveryName = "DEL01", TransactionCode = "INV01" }
        };
        var date = DateTime.Today;
        _mockRepo.Setup(r => r.GetDividedInvoicesByDeliveryIdAndDateAsync("TEST_DB", "DEL01", date))
            .ReturnsAsync(mockData);

        var result = await _controller.GetDividedInvoiceByDeliverIdAndDate("DEL01", date);

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Single(result.Result);
    }

    [Fact]
    public async Task GetDividedInvoiceByDeliverIdAndDate_ShouldReturnBadRequest_WhenEmpty()
    {
        var date = DateTime.Today;
        _mockRepo.Setup(r => r.GetDividedInvoicesByDeliveryIdAndDateAsync("TEST_DB", "DEL01", date))
            .ReturnsAsync(new List<DividedInvoiceResponse>());

        var result = await _controller.GetDividedInvoiceByDeliverIdAndDate("DEL01", date);

        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Empty(result.Result);
    }

    [Fact]
    public async Task DeleteDividedInvoiceByDeliverIdAndDate_ShouldReturnOk_WhenSuccessful()
    {
        var model = new DeleteDividedInvoiceRequest
        {
            InvoiceId = 1,
            TransactionCode = "INV01",
            DeliveryId = "DEL01",
            Note = "Note"
        };
        _mockRepo.Setup(r => r.DeleteDividedInvoiceAsync(1, "INV01", "DEL01", "Note"))
            .ReturnsAsync(1);

        var result = await _controller.DeleteDividedInvoiceByDeliverIdAndDate(model);

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task DeleteDividedInvoiceByDeliverIdAndDate_ShouldReturnBadRequest_WhenFailed()
    {
        var model = new DeleteDividedInvoiceRequest
        {
            InvoiceId = 1,
            TransactionCode = "INV01",
            DeliveryId = "DEL01",
            Note = "Note"
        };
        _mockRepo.Setup(r => r.DeleteDividedInvoiceAsync(1, "INV01", "DEL01", "Note"))
            .ReturnsAsync(0);

        var result = await _controller.DeleteDividedInvoiceByDeliverIdAndDate(model);

        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task GetInvoiceByAreaAndTransCodeAsync_ShouldReturnOk_WhenFound()
    {
        var mockData = new List<InvoiceResponse>
        {
            new() { CustomerCode = "C01", InvoiceCode = "T01" }
        };
        _mockInvoiceRepo.Setup(r => r.GetInvoiceByAreaAndTransCodeAsync("TEST_DB", "A01", "T01"))
            .ReturnsAsync(mockData);

        var result = await _controller.GetInvoiceByAreaAndTransCodeAsync("A01", "T01");

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Single(result.Result);
    }

    [Fact]
    public async Task GetInvoiceByAreaAndTransCodeAsync_ShouldReturnBadRequest_WhenEmpty()
    {
        _mockInvoiceRepo.Setup(r => r.GetInvoiceByAreaAndTransCodeAsync("TEST_DB", "A01", "T01"))
            .ReturnsAsync(new List<InvoiceResponse>());

        var result = await _controller.GetInvoiceByAreaAndTransCodeAsync("A01", "T01");

        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Empty(result.Result);
    }

    [Fact]
    public async Task SaveDividedInvoiceAsync_ShouldReturnOk_WhenSavedSuccessfully()
    {
        var requests = new List<CreateDividedInvoiceRequest>
        {
            new() { InvoiceId = 1, DeliveryId = "DEL01" }
        };
        _mockRepo.Setup(r => r.SaveDividedInvoiceAsync(requests, "TEST_DB", "TEST_USER"))
            .ReturnsAsync(1);

        var result = await _controller.SaveDividedInvoiceAsync(requests);

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(1, result.Result);
    }

    [Fact]
    public async Task SaveDividedInvoiceAsync_ShouldReturnBadRequest_WhenFailed()
    {
        var requests = new List<CreateDividedInvoiceRequest>
        {
            new() { InvoiceId = 1, DeliveryId = "DEL01" }
        };
        _mockRepo.Setup(r => r.SaveDividedInvoiceAsync(requests, "TEST_DB", "TEST_USER"))
            .ReturnsAsync(0);

        var result = await _controller.SaveDividedInvoiceAsync(requests);

        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task GetDividedInvoiceStatusByDateAsync_ShouldReturnOk_WhenFound()
    {
        var mockData = new List<DividedInvoiceStatusResponse>
        {
            new() { Id = 1, Delivery = "DEL01", TransactionCode = "INV01", InvoiceValue = 100 }
        };
        var date = DateTime.Today;
        var deliveryId = "DEL01";
        _mockRepo.Setup(r => r.GetDividedInvoiceStatusByDateAsync("TEST_DB", date, deliveryId))
            .ReturnsAsync(mockData);

        var result = await _controller.GetDividedInvoiceStatusByDateAsync(date, deliveryId);

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.True(result.Success);
        Assert.NotNull(result.Result);
        Assert.Single(result.Result);
    }

    [Fact]
    public async Task GetDividedInvoiceStatusByDateAsync_ShouldReturnBadRequest_WhenEmpty()
    {
        var date = DateTime.Today;
        var deliveryId = "DEL01";
        _mockRepo.Setup(r => r.GetDividedInvoiceStatusByDateAsync("TEST_DB", date, deliveryId))
            .ReturnsAsync(new List<DividedInvoiceStatusResponse>());

        var result = await _controller.GetDividedInvoiceStatusByDateAsync(date, deliveryId);

        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Empty(result.Result);
    }
}