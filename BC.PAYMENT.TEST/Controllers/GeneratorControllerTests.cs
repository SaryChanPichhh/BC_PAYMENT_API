using BC.PAYMENT.API.Controllers.General;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.APPLICATION.Interfaces.Generator;
using BC.PAYMENT.CORE.DTO.Generator;
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

public class GeneratorControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IGeneratorRepository> _mockGeneratorRepo;
    private readonly GeneratorController _controller;

    public GeneratorControllerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockGeneratorRepo = new Mock<IGeneratorRepository>();

        _mockUnitOfWork.Setup(u => u.Generators).Returns(_mockGeneratorRepo.Object);

        _controller = new GeneratorController(_mockUnitOfWork.Object);

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
    public async Task GetSaleAnalysisByCustomerCodeAsync_ShouldReturnOk_WhenFound()
    {
        var mockData = new SaleAnalysisDto
        {
            AnalysisC0 = "C0",
            AnalysisC1 = "C1"
        };

        _mockGeneratorRepo.Setup(r => r.GetSaleAnalysisByCustomerCodeAsync("CUST01", "TEST_DB"))
            .ReturnsAsync(mockData);

        var result = await _controller.GetSaleAnalysisByCustomerCodeAsync("CUST01");

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.True(result.Success);
        Assert.NotNull(result.Result);
        Assert.Equal("C0", result.Result.AnalysisC0);
        Assert.Equal("Sale analysis fetched successfully", result.Message);
    }

    [Fact]
    public async Task GetSaleAnalysisByCustomerCodeAsync_ShouldReturnBadRequest_WhenNull()
    {
        _mockGeneratorRepo.Setup(r => r.GetSaleAnalysisByCustomerCodeAsync("CUST01", "TEST_DB"))
            .ReturnsAsync((SaleAnalysisDto)null!);

        var result = await _controller.GetSaleAnalysisByCustomerCodeAsync("CUST01");

        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        Assert.False(result.Success);
        Assert.Equal("Sale analysis fetched unsuccessfully", result.Message);
    }

    [Fact]
    public async Task GenerateAdjRefCodeAsync_ShouldReturnOk_WhenGenerated()
    {
        _mockGeneratorRepo.Setup(r => r.GenerateAdjRefCode("TEST_DB", "ADJ", "M"))
            .ReturnsAsync("ADJ26090001");

        var result = await _controller.GenerateAdjRefCodeAsync("ADJ", "M");

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.True(result.Success);
        Assert.Equal("ADJ26090001", result.Result);
        Assert.Equal("Adjustment reference code generated successfully", result.Message);
    }

    [Fact]
    public async Task GenerateFixInvoiceAsync_ShouldReturnOk_WhenGenerated()
    {
        _mockGeneratorRepo.Setup(r => r.GenerateFixInvoice("TEST_DB"))
            .ReturnsAsync("FF26090001");

        var result = await _controller.GenerateFixInvoiceAsync();

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.True(result.Success);
        Assert.Equal("FF26090001", result.Result);
        Assert.Equal("Fix invoice generated successfully", result.Message);
    }

    [Fact]
    public async Task PostSaleOrderAutoNumberAsync_ShouldReturnOk_WhenGenerated()
    {
        _mockGeneratorRepo.Setup(r => r.PostSaleOrderAutoNumberAsync("SALE01", "TEST_DB"))
            .ReturnsAsync("SO26090001");

        var result = await _controller.PostSaleOrderAutoNumberAsync("SALE01");

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.True(result.Success);
        Assert.Equal("SO26090001", result.Result);
        Assert.Equal("Sale order auto number generated successfully", result.Message);
    }

    [Fact]
    public async Task PostCreditNoteAutoNumberAsync_ShouldReturnOk_WhenGenerated()
    {
        _mockGeneratorRepo.Setup(r => r.PostCreditNoteAutoNumberAsync("TEST_DB", "CN01"))
            .ReturnsAsync("CN26090001");

        var result = await _controller.PostCreditNoteAutoNumberAsync("CN01");

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.True(result.Success);
        Assert.Equal("CN26090001", result.Result);
        Assert.Equal("Credit note auto number generated successfully", result.Message);
    }

    [Fact]
    public async Task GetSaleCodeAsync_ShouldReturnOk_WhenFound()
    {
        var mockCodes = new List<string> { "SALE-EXCH", "SALE-REP" };
        _mockGeneratorRepo.Setup(r => r.GetSaleCodeAsync("TEST_DB"))
            .ReturnsAsync(mockCodes);

        var result = await _controller.GetSaleCodeAsync();

        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.True(result.Success);
        Assert.Equal(2, result.Result.Count);
        Assert.Equal("Sale codes fetched successfully", result.Message);
    }
}