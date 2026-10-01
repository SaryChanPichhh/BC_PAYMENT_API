using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using BC.PAYMENT.CORE.Entities;
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

public class ReturnInvoiceRepositoryTests
{
    private readonly Mock<ISqlDataAccess> _mockSqlDataAccess;
    private readonly ReturnInvoiceRepository _repository;

    public ReturnInvoiceRepositoryTests()
    {
        _mockSqlDataAccess = new Mock<ISqlDataAccess>();
        _repository = new ReturnInvoiceRepository(_mockSqlDataAccess.Object);
    }

    [Fact]
    public async Task GetReturnInvoiceAsync_ShouldReturnList()
    {
        var mockData = new List<ReturnInvoiceResponse>
        {
            new() { InvoiceId = 1, TransactionCode = "INV01", CustomerCode = "C01", InvoiceValue = 100 }
        };
        _mockSqlDataAccess
            .Setup(db => db.LoadData<ReturnInvoiceResponse, dynamic>(ReturnInvoiceQueries.GetReturnInvoice,
                It.IsAny<object>(), CommandType.Text, "Default"))
            .ReturnsAsync(mockData);

        var result = await _repository.GetReturnInvoiceAsync("TEST_DB");

        Assert.Single(result);
        Assert.Equal("INV01", result[0].TransactionCode);
    }

    [Fact]
    public async Task GetReturnInvoiceByDateAsync_ShouldReturnList()
    {
        var mockData = new List<ReturnInvoiceResponse>
        {
            new() { InvoiceId = 2, TransactionCode = "INV02", CustomerCode = "C02", InvoiceValue = 200 }
        };
        _mockSqlDataAccess
            .Setup(db =>
                db.LoadData<ReturnInvoiceResponse, dynamic>(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text,
                    "Default"))
            .ReturnsAsync(mockData);

        var result =
            await _repository.GetReturnInvoiceByDateAsync("TEST_DB", DateTime.Today.AddDays(-1), DateTime.Today);

        Assert.Single(result);
        Assert.Equal("INV02", result[0].TransactionCode);
    }

    [Fact]
    public async Task SaveReturnInvoiceAsync_ShouldReturnRowsAffected()
    {
        var requests = new List<ReturnInvoiceRequest>
        {
            new() { TransactionCode = "INV01", CustomerCode = "C01", CustomerName = "Customer A", InvoiceValue = 150 },
            new() { TransactionCode = "INV02", CustomerCode = "C02", CustomerName = "Customer B", InvoiceValue = 250 }
        };
        _mockSqlDataAccess
            .Setup(db => db.ExecuteAsync(ReturnInvoiceQueries.InsertReturnInvoice, It.IsAny<object>(), CommandType.Text,
                "Default"))
            .ReturnsAsync(1);

        var result = await _repository.SaveReturnInvoiceAsync(requests, "TEST_DB", "TEST_USER", "ENTRY_01");

        Assert.Equal(2, result);
    }

    [Fact]
    public async Task CreatePcReturnInvoiceAsync_WithModel_ShouldReturnRowsAffected()
    {
        var model = new PcReturnInvoice
        {
            DbCode = "TEST_DB",
            DividedId = 10,
            HeaderId = 20,
            Description = "Damaged product",
            CreatedBy = "TEST_USER"
        };

        _mockSqlDataAccess
            .Setup(db => db.ExecuteAsync(ReturnInvoiceQueries.CreatePcReturnInvoice, It.IsAny<object>(),
                CommandType.Text, "Default"))
            .ReturnsAsync(1);

        var result = await _repository.CreatePcReturnInvoiceAsync(model);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task CreatePcReturnInvoiceAsync_WithList_ShouldReturnRowsAffected()
    {
        var models = new List<PcReturnInvoice>
        {
            new() { DbCode = "TEST_DB", DividedId = 10, HeaderId = 20, Description = "Desc 1", CreatedBy = "USER" },
            new() { DbCode = "TEST_DB", DividedId = 11, HeaderId = 20, Description = "Desc 2", CreatedBy = "USER" }
        };

        _mockSqlDataAccess
            .Setup(db => db.ExecuteAsync(ReturnInvoiceQueries.CreatePcReturnInvoice, It.IsAny<object>(),
                CommandType.Text, "Default"))
            .ReturnsAsync(1);

        var result = await _repository.CreatePcReturnInvoiceAsync(models);

        Assert.Equal(2, result);
    }

    [Fact]
    public async Task DeletePcReturnInvoiceAsync_ShouldReturnCount_WhenDeleteSucceeds()
    {
        _mockSqlDataAccess
            .Setup(db => db.ExecuteAsync(ReturnInvoiceQueries.DeletePcReturnInvoice, It.IsAny<object>(),
                CommandType.Text, "Default"))
            .ReturnsAsync(1);
        _mockSqlDataAccess
            .Setup(db => db.ExecuteAsync(ReturnInvoiceQueries.UpdateDividedInvoiceStatusAfterReturnDelete,
                It.IsAny<object>(), CommandType.Text, "Default"))
            .ReturnsAsync(1);

        var result = await _repository.DeletePcReturnInvoiceAsync(10);

        Assert.Equal(1, result);
        _mockSqlDataAccess.Verify(
            db => db.ExecuteAsync(ReturnInvoiceQueries.DeletePcReturnInvoice, It.IsAny<object>(), CommandType.Text,
                "Default"), Times.Once);
        _mockSqlDataAccess.Verify(
            db => db.ExecuteAsync(ReturnInvoiceQueries.UpdateDividedInvoiceStatusAfterReturnDelete, It.IsAny<object>(),
                CommandType.Text, "Default"), Times.Once);
    }

    [Fact]
    public async Task DeletePcReturnInvoiceAsync_ShouldNotUpdate_WhenDeleteReturnsZero()
    {
        _mockSqlDataAccess
            .Setup(db => db.ExecuteAsync(ReturnInvoiceQueries.DeletePcReturnInvoice, It.IsAny<object>(),
                CommandType.Text, "Default"))
            .ReturnsAsync(0);

        var result = await _repository.DeletePcReturnInvoiceAsync(10);

        Assert.Equal(0, result);
        _mockSqlDataAccess.Verify(
            db => db.ExecuteAsync(ReturnInvoiceQueries.DeletePcReturnInvoice, It.IsAny<object>(), CommandType.Text,
                "Default"), Times.Once);
        _mockSqlDataAccess.Verify(
            db => db.ExecuteAsync(ReturnInvoiceQueries.UpdateDividedInvoiceStatusAfterReturnDelete, It.IsAny<object>(),
                CommandType.Text, "Default"), Times.Never);
    }
}