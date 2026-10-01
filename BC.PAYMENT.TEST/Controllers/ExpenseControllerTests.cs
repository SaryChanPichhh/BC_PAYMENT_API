using BC.PAYMENT.API.Controllers.Expense;
using BC.PAYMENT.APPLICATION.Interfaces.Expense;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Contracts.Request.Expense;
using BC.PAYMENT.CORE.Contracts.Response.Expense;
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

public class ExpenseControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IExpenseRepository> _mockExpenseRepo;
    private readonly ExpenseController _controller;

    public ExpenseControllerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockExpenseRepo = new Mock<IExpenseRepository>();

        _mockUnitOfWork.Setup(u => u.Expense).Returns(_mockExpenseRepo.Object);

        _controller = new ExpenseController(_mockUnitOfWork.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("DbCode", "TEST_DB"),
            new Claim("Username", "TEST_USER")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task LoadBcPaymentDetailByDateRangeAsync_ShouldReturnOk_WhenResultsExist()
    {
        var fromDate = new DateTime(2024, 5, 1);
        var toDate = new DateTime(2024, 5, 10);
        var mockData = new List<BcPaymentDetailResponse>
        {
            new() { Id = 1, DbCode = "TEST_DB", Total = 100 }
        };

        _mockExpenseRepo
            .Setup(r => r.LoadBcPaymentDetailAsync("TEST_DB", fromDate, toDate))
            .ReturnsAsync(mockData);

        var response = await _controller.LoadBcPaymentDetailByDateRangeAsync(fromDate, toDate);

        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Success);
        Assert.Single(response.Result);
    }

    [Fact]
    public async Task LoadBcPaymentDetailByDateRangeAsync_ShouldReturnBadRequest_WhenEmpty()
    {
        var fromDate = new DateTime(2024, 5, 1);
        var toDate = new DateTime(2024, 5, 10);

        _mockExpenseRepo
            .Setup(r => r.LoadBcPaymentDetailAsync("TEST_DB", fromDate, toDate))
            .ReturnsAsync(new List<BcPaymentDetailResponse>());

        var response = await _controller.LoadBcPaymentDetailByDateRangeAsync(fromDate, toDate);

        Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task LoadBcPaymentDetailByPeriodAsync_ShouldReturnOk_WhenResultsExist()
    {
        var month = 5;
        var year = 2024;
        var mockData = new List<BcPaymentDetailResponse>
        {
            new() { Id = 2, DbCode = "TEST_DB", Total = 200 }
        };

        _mockExpenseRepo
            .Setup(r => r.LoadBcPaymentDetailAsync("TEST_DB", month, year))
            .ReturnsAsync(mockData);

        var response = await _controller.LoadBcPaymentDetailByPeriodAsync(month, year);

        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Success);
        Assert.Single(response.Result);
    }

    [Fact]
    public async Task LoadBcPaymentDetailByPeriodAsync_ShouldReturnBadRequest_WhenEmpty()
    {
        var month = 5;
        var year = 2024;

        _mockExpenseRepo
            .Setup(r => r.LoadBcPaymentDetailAsync("TEST_DB", month, year))
            .ReturnsAsync(new List<BcPaymentDetailResponse>());

        var response = await _controller.LoadBcPaymentDetailByPeriodAsync(month, year);

        Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task UpdateBcPaymentDetailAsync_ShouldReturnOk_WhenUpdateSucceeds()
    {
        var request = new UpdateBcPaymentDetailRequest
        {
            Id = 1,
            Total = 500,
            Dollar = 100,
            Riel = 400000,
            Exchange = 4000,
            DescExp1 = "Desc1",
            ExpAmount1 = 50,
            MoneyBias = 10
        };

        _mockExpenseRepo
            .Setup(r => r.UpdateBcPaymentDetailAsync(request))
            .ReturnsAsync(1);

        var response = await _controller.UpdateBcPaymentDetailAsync(request);

        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Success);
        Assert.True(response.Result);
    }

    [Fact]
    public async Task UpdateBcPaymentDetailAsync_ShouldReturnBadRequest_WhenUpdateFails()
    {
        var request = new UpdateBcPaymentDetailRequest { Id = 99 };

        _mockExpenseRepo
            .Setup(r => r.UpdateBcPaymentDetailAsync(request))
            .ReturnsAsync(0);

        var response = await _controller.UpdateBcPaymentDetailAsync(request);

        Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(response.Success);
        Assert.False(response.Result);
    }
}