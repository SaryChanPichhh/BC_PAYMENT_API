using BC.PAYMENT.CORE.Contracts.Request.Invoice;
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

public class DividedInvoiceRepositoryTests
{
    private readonly Mock<ISqlDataAccess> _mockSqlDataAccess;
    private readonly DividedInvoiceRepository _repository;

    public DividedInvoiceRepositoryTests()
    {
        _mockSqlDataAccess = new Mock<ISqlDataAccess>();
        _repository = new DividedInvoiceRepository(_mockSqlDataAccess.Object);
    }

    [Fact]
    public async Task CheckExistsDividedInvoice_ShouldReturnTrue_WhenExists()
    {
        _mockSqlDataAccess
            .Setup(db => db.LoadSingleData<bool, dynamic>(DividedInvoiceQueries.CheckExistsDividedInvoice,
                It.IsAny<object>(), CommandType.Text, "Default"))
            .ReturnsAsync(true);

        var result = await _repository.CheckExistsDividedInvoice(100);

        Assert.True(result);
    }

    [Fact]
    public async Task CheckExistsDividedInvoice_ShouldReturnFalse_WhenNotExists()
    {
        _mockSqlDataAccess
            .Setup(db => db.LoadSingleData<bool, dynamic>(DividedInvoiceQueries.CheckExistsDividedInvoice,
                It.IsAny<object>(), CommandType.Text, "Default"))
            .ReturnsAsync(false);

        var result = await _repository.CheckExistsDividedInvoice(100);

        Assert.False(result);
    }

    [Fact]
    public async Task SaveDividedInvoiceAsync_ShouldSkipExistingAndSaveNew()
    {
        var requests = new List<CreateDividedInvoiceRequest>
        {
            new() { InvoiceId = 1, DeliveryId = "DEL01", CreatedDate = DateTime.Today },
            new() { InvoiceId = 2, DeliveryId = "DEL02", CreatedDate = DateTime.Today }
        };

        _mockSqlDataAccess
            .Setup(db => db.LoadSingleData<bool, dynamic>(DividedInvoiceQueries.CheckExistsDividedInvoice,
                It.Is<object>(o => o.ToString()!.Contains("1")), CommandType.Text, "Default"))
            .ReturnsAsync(true);
        _mockSqlDataAccess
            .Setup(db => db.LoadSingleData<bool, dynamic>(DividedInvoiceQueries.CheckExistsDividedInvoice,
                It.Is<object>(o => o.ToString()!.Contains("2")), CommandType.Text, "Default"))
            .ReturnsAsync(false);

        _mockSqlDataAccess
            .Setup(db => db.ExecuteAsync(DividedInvoiceQueries.InsertDividedInvoice, It.IsAny<object>(),
                CommandType.Text, "Default"))
            .ReturnsAsync(1);
        _mockSqlDataAccess
            .Setup(db => db.ExecuteAsync(DividedInvoiceQueries.UpdateNewInvoiceDividedStatus, It.IsAny<object>(),
                CommandType.Text, "Default"))
            .ReturnsAsync(1);

        var result = await _repository.SaveDividedInvoiceAsync(requests, "TEST_DB", "TEST_USER");

        Assert.Equal(1, result);
    }
}