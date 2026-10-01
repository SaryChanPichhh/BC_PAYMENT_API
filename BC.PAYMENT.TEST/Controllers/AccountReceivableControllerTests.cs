using BC.PAYMENT.API.Controllers.Ar;
using BC.PAYMENT.APPLICATION.Interfaces.Accounting;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Contracts.Request.AccountReceivable;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace BC.PAYMENT.TEST.Controllers;

public class AccountReceivableControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IAccountReceivableRepository> _mockArRepo;
    private readonly AccountReceivableController _controller;

    public AccountReceivableControllerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockArRepo = new Mock<IAccountReceivableRepository>();

        _mockUnitOfWork.Setup(u => u.AccountReceivable).Returns(_mockArRepo.Object);

        _controller = new AccountReceivableController(_mockUnitOfWork.Object);

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
    public async Task SaveLedgerAsync_ShouldReturnOk_WhenSuccessful()
    {
        var request = new SiLedgerRequest
        {
            ACC_CODE = "CUST01",
            AMOUNT_6 = 100
        };

        _mockArRepo.Setup(r => r.InsertAccountReceivable(It.IsAny<SiLedgerRequest>(), false))
            .ReturnsAsync(2);

        var result = await _controller.SaveLedgerAsync(request, false);

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.True(result.Success);
        Assert.Equal(2, result.Result);
        Assert.Equal("Ledger saved successfully", result.Message);
    }

    [Fact]
    public async Task SaveLedgerAsync_ShouldReturnBadRequest_WhenFailed()
    {
        var request = new SiLedgerRequest
        {
            ACC_CODE = "CUST01",
            AMOUNT_6 = 100
        };

        _mockArRepo.Setup(r => r.InsertAccountReceivable(It.IsAny<SiLedgerRequest>(), false))
            .ReturnsAsync(0);

        var result = await _controller.SaveLedgerAsync(request, false);

        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        Assert.False(result.Success);
        Assert.Equal(0, result.Result);
        Assert.Equal("Ledger saved unsuccessfully", result.Message);
    }
}