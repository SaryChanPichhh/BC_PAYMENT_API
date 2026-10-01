using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Invoice;
using BC.PAYMENT.SQL.Queries;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace BC.PAYMENT.TEST.Repositories;

public class ChangeInvoiceRepositoryTests
{
    private readonly Mock<ISqlDataAccess> _mockSqlDataAccess;
    private readonly ChangeInvoiceRepository _repository;

    public ChangeInvoiceRepositoryTests()
    {
        _mockSqlDataAccess = new Mock<ISqlDataAccess>();
        _repository = new ChangeInvoiceRepository(_mockSqlDataAccess.Object);
    }

    [Fact]
    public async Task GetChangeInvoicesAsync_ShouldReturnList()
    {
        var mockData = new List<ChangeInvoiceResponse>
        {
            new() { InvoiceId = 1, InvoiceCode = "INV01", CustomerCode = "C01", InvoiceAmount = 100 }
        };
        _mockSqlDataAccess
            .Setup(db => db.LoadData<ChangeInvoiceResponse, dynamic>(ChangeInvoiceQueries.GetChangeInvoices,
                It.IsAny<object>(), CommandType.Text, "Default"))
            .ReturnsAsync(mockData);

        var result = await _repository.GetChangeInvoicesAsync("TEST_DB", DateTime.Today);

        Assert.Single(result);
        Assert.Equal("INV01", result[0].InvoiceCode);
    }

    [Fact]
    public async Task CheckExistInvoiceAsync_ShouldReturnBool()
    {
        _mockSqlDataAccess
            .Setup(db => db.LoadSingleData<bool, dynamic>(ChangeInvoiceQueries.CheckExistInvoice, It.IsAny<object>(),
                CommandType.Text, "Default"))
            .ReturnsAsync(true);

        var result = await _repository.CheckExistInvoiceAsync("INV01", "TEST_DB");

        Assert.True(result);
    }

    [Fact]
    public async Task GetLocalInvoiceAsync_ShouldReturnList()
    {
        var mockData = new List<ChangeInvoiceResponse>
        {
            new() { InvoiceCode = "LOC01", CustomerCode = "C01", InvoiceAmount = 150 }
        };
        _mockSqlDataAccess
            .Setup(db => db.LoadData<ChangeInvoiceResponse, dynamic>(ChangeInvoiceQueries.GetLocalInvoice,
                It.IsAny<object>(), CommandType.Text, "Default"))
            .ReturnsAsync(mockData);

        var result = await _repository.GetLocalInvoiceAsync("TEST_DB", DateTime.Today.AddDays(-1), DateTime.Today);

        Assert.Single(result);
        Assert.Equal("LOC01", result[0].InvoiceCode);
    }

    [Fact]
    public async Task GetOtherBranchInvoiceAsync_ShouldReturnList()
    {
        var mockData = new List<ChangeInvoiceResponse>
        {
            new() { InvoiceCode = "OTH01", CustomerCode = "C02", InvoiceAmount = 200 }
        };
        _mockSqlDataAccess
            .Setup(db => db.LoadData<ChangeInvoiceResponse, dynamic>(ChangeInvoiceQueries.GetOtherBranchInvoice,
                It.IsAny<object>(), CommandType.Text, "Default"))
            .ReturnsAsync(mockData);

        var result =
            await _repository.GetOtherBranchInvoiceAsync("TEST_DB", DateTime.Today.AddDays(-1), DateTime.Today);

        Assert.Single(result);
        Assert.Equal("OTH01", result[0].InvoiceCode);
    }

    [Fact]
    public async Task AddChangeInvoiceAsync_ShouldExecuteUpdate_WhenIsExistsTrue()
    {
        var model = new ChangeInvoiceModel
        {
            IsExists = true,
            DbCode = "TEST_DB",
            Transaction = "INV01",
            CustomerCode = "C01",
            CustomerName = "Customer A",
            InvoiceValue = 100,
            EntriesCode = "ENTRY01",
            UserName = "TEST_USER"
        };
        _mockSqlDataAccess
            .Setup(db => db.ExecuteAsync(ChangeInvoiceQueries.UpdateExistInvoice, It.IsAny<object>(), CommandType.Text,
                "Default"))
            .ReturnsAsync(1);

        var result = await _repository.AddChangeInvoiceAsync(model);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task AddChangeInvoiceAsync_ShouldExecuteInsert_WhenIsExistsFalse()
    {
        var model = new ChangeInvoiceModel
        {
            IsExists = false,
            DbCode = "TEST_DB",
            Transaction = "INV01",
            CustomerCode = "C01",
            CustomerName = "Customer A",
            InvoiceValue = 100,
            EntriesCode = "ENTRY01",
            UserName = "TEST_USER"
        };
        _mockSqlDataAccess
            .Setup(db => db.ExecuteAsync(ChangeInvoiceQueries.InsertIfNotExistsInvoice, It.IsAny<object>(),
                CommandType.Text, "Default"))
            .ReturnsAsync(1);

        var result = await _repository.AddChangeInvoiceAsync(model);

        Assert.Equal(1, result);
    }
}