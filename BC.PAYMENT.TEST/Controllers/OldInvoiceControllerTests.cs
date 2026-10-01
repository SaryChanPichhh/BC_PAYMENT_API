using BC.PAYMENT.API.Controllers.Invoice;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.APPLICATION.Interfaces.Invoice;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.AccountReceivable;
using BC.PAYMENT.CORE.Contracts.Criteria;
using BC.PAYMENT.CORE.Contracts.Invoice;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using BC.PAYMENT.CORE.Contracts.TablesType;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.AccountReceivable;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.AccountReceivable;
using Xunit;

namespace BC.PAYMENT.TEST.Controllers;

public class OldInvoiceControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IOldInvoiceRepository> _mockOldInvoiceRepo;
    private readonly Mock<IConfirmAccountReceivableRepository> _mockConfirmRepo;
    private readonly OldInvoiceController _controller;

    public OldInvoiceControllerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockOldInvoiceRepo = new Mock<IOldInvoiceRepository>();
        _mockConfirmRepo = new Mock<IConfirmAccountReceivableRepository>();

        _mockUnitOfWork.Setup(u => u.OldInvoice).Returns(_mockOldInvoiceRepo.Object);
        _mockUnitOfWork.Setup(u => u.ConfirmAccountReceivable).Returns(_mockConfirmRepo.Object);

        _controller = new OldInvoiceController(_mockUnitOfWork.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("DbCode", "TEST_DB"),
            new Claim("Username", "TEST_USER"),
            new Claim("InvoiceEntryCode", "ENTRY_01"),
            new Claim("Period", "202609"),
            new Claim("CurrentDate", DateTime.Today.ToString("MM/dd/yyyy"))
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GetOldInvoiceAsync_ShouldReturnOk_WhenInvoicesExist()
    {
        var mockData = new List<OldInvoiceResponse>
        {
            new() { TransactionCode = "OLD01", CustomerCode = "C01", InvoiceValue = 100 }
        };
        _mockOldInvoiceRepo.Setup(r => r.GetOldInvoiceAsync("TEST_DB", 1, 10, null)).ReturnsAsync(mockData);

        var result = await _controller.GetOldInvoiceAsync(1, 10);

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Single(result.Result.Data);
    }

    [Fact]
    public async Task GetOldInvoiceAsync_ShouldReturnBadRequest_WhenEmpty()
    {
        _mockOldInvoiceRepo.Setup(r => r.GetOldInvoiceAsync("TEST_DB", 1, 10, null))
            .ReturnsAsync(new List<OldInvoiceResponse>());

        var result = await _controller.GetOldInvoiceAsync(1, 10);

        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task GetOldInvoiceFromLedgerAsync_Criteria_ShouldReturnOk_WhenInvoicesExist()
    {
        var criteria = new OldInvoiceCriteria { DbCode = "TEST_DB" };
        var mockData = new List<OldInvoiceResponse>
        {
            new() { TransactionCode = "OLD01", CustomerCode = "C01" }
        };
        _mockOldInvoiceRepo.Setup(r => r.GetAllOldInvoicesFromLedgerAsync(criteria)).ReturnsAsync(mockData);

        var result = await _controller.GetOldInvoiceFromLedgerAsync(criteria);

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Single(result.Result);
    }

    [Fact]
    public async Task GetOldInvoiceFromLedgerAsync_Criteria_ShouldReturnBadRequest_WhenEmpty()
    {
        var criteria = new OldInvoiceCriteria { DbCode = "TEST_DB" };
        _mockOldInvoiceRepo.Setup(r => r.GetAllOldInvoicesFromLedgerAsync(criteria))
            .ReturnsAsync(new List<OldInvoiceResponse>());

        var result = await _controller.GetOldInvoiceFromLedgerAsync(criteria);

        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task GetOldInvoiceByTransCode_ShouldReturnBadRequest_WhenFound()
    {
        var invoice = new OldInvoiceResponse { TransactionCode = "OLD01" };
        _mockOldInvoiceRepo.Setup(r => r.GetOldInvoiceByTransactionCode("TEST_DB", "OLD01")).ReturnsAsync(invoice);

        var result = await _controller.GetOldInvoiceFromLedgerAsync("OLD01");

        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal("OLD01", result.Result.TransactionCode);
    }

    [Fact]
    public async Task SaveOldInvoiceAsync_ShouldReturnOk_WhenSuccessful()
    {
        var request = new List<OldInvoiceRequest>
        {
            new("OLD01", "C01", "John", 50, "EMP1")
        };
        _mockOldInvoiceRepo.Setup(r => r.SaveOldInvoice(It.IsAny<List<OldInvoiceTableType>>())).ReturnsAsync(1);

        var result = await _controller.SaveOldInvoiceAsync(request);

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(1, result.Result);
    }

    [Fact]
    public async Task SaveOldInvoiceAsync_ShouldReturnNotFound_WhenFailed()
    {
        var request = new List<OldInvoiceRequest>
        {
            new("OLD01", "C01", "John", 50, "EMP1")
        };
        _mockOldInvoiceRepo.Setup(r => r.SaveOldInvoice(It.IsAny<List<OldInvoiceTableType>>())).ReturnsAsync(0);

        var result = await _controller.SaveOldInvoiceAsync(request);

        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task CreateNewInvoiceFromOldInvoiceAsync_ShouldReturnOk_WhenSuccessful()
    {
        _mockOldInvoiceRepo.Setup(r => r.RecreateOldInvoiceAsync(It.IsAny<InvoiceDTO>())).ReturnsAsync(1);
        _mockOldInvoiceRepo.Setup(r => r.GetOldInvoiceByTransactionCode("TEST_DB", "OLD01"))
            .ReturnsAsync(new OldInvoiceResponse { TransactionCode = "OLD01" });

        var result = await _controller.CreateNewInvoiceFromOldInvoiceAsync("OLD01");

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal("OLD01", result.Result.TransactionCode);
    }

    [Fact]
    public async Task CreateNewInvoiceFromOldInvoiceAsync_ShouldReturnNotFound_WhenRowsAffectedZero()
    {
        _mockOldInvoiceRepo.Setup(r => r.RecreateOldInvoiceAsync(It.IsAny<InvoiceDTO>())).ReturnsAsync(0);

        var result = await _controller.CreateNewInvoiceFromOldInvoiceAsync("OLD01");

        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task DeleteOldInvoiceAsync_ShouldReturnOk_WhenSuccessful()
    {
        _mockOldInvoiceRepo.Setup(r => r.DeleteOldInvoiceByIdAsync(1)).ReturnsAsync(1);

        var result = await _controller.DeleteOldInvoiceAsync(1);

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(1, result.Result);
    }

    [Fact]
    public async Task DeleteOldInvoiceAsync_ShouldReturnNotFound_WhenFailed()
    {
        _mockOldInvoiceRepo.Setup(r => r.DeleteOldInvoiceByIdAsync(1)).ReturnsAsync(0);

        var result = await _controller.DeleteOldInvoiceAsync(1);

        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
    }
}