using BC.PAYMENT.CORE.Contracts.Response.Expense;
using BC.PAYMENT.CORE.Entities;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Invoice;
using Dapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BC.PAYMENT.TEST.Repositories
{
    public class PaymentInvoiceRepositoryTests
    {
        private readonly Mock<ISqlDataAccess> _mockSqlDataAccess;
        private readonly PaymentInvoiceRepository _repository;

        public PaymentInvoiceRepositoryTests()
        {
            _mockSqlDataAccess = new Mock<ISqlDataAccess>();
            _repository = new PaymentInvoiceRepository(_mockSqlDataAccess.Object);
        }

        [Fact]
        public async Task LoadBcPaymentDetailAsync_WithSingleDate_ShouldIncludeDateFilter()
        {
            string capturedSql = string.Empty;
            DynamicParameters? capturedParams = null;

            _mockSqlDataAccess
                .Setup(db => db.LoadData<BcPaymentDetailResponse, dynamic>(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
                .Callback<string, object, CommandType, string>((sql, param, cmdType, conn) =>
                {
                    capturedSql = sql;
                    capturedParams = param as DynamicParameters;
                })
                .ReturnsAsync(new List<BcPaymentDetailResponse>
                {
                    new() { Id = 1, DbCode = "TEST_DB", DeliveryId = "DEL01", Total = 100 }
                });

            var targetDate = new DateTime(2024, 5, 1);
            var result = await _repository.LoadBcPaymentDetailAsync("TEST_DB", "DEL01", date: targetDate);

            Assert.Single(result);
            Assert.Equal("DEL01", result[0].DeliveryId);
            Assert.Contains("DELIVERYID = @DELIVERY_ID", capturedSql);
            Assert.Contains("CREATED_DATE >= @DATE AND CREATED_DATE < DATEADD(DAY, 1, @DATE)", capturedSql);
            Assert.NotNull(capturedParams);
            Assert.Equal("TEST_DB", capturedParams.Get<string>("DB_CODE"));
            Assert.Equal("DEL01", capturedParams.Get<string>("DELIVERY_ID"));
            Assert.Equal(targetDate.Date, capturedParams.Get<DateTime>("DATE"));
        }

        [Fact]
        public async Task LoadBcPaymentDetailAsync_WithDateRange_ShouldIncludeRangeFilter()
        {
            string capturedSql = string.Empty;
            DynamicParameters? capturedParams = null;

            _mockSqlDataAccess
                .Setup(db => db.LoadData<BcPaymentDetailResponse, dynamic>(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
                .Callback<string, object, CommandType, string>((sql, param, cmdType, conn) =>
                {
                    capturedSql = sql;
                    capturedParams = param as DynamicParameters;
                })
                .ReturnsAsync(new List<BcPaymentDetailResponse>
                {
                    new() { Id = 2, DbCode = "TEST_DB", Total = 200 }
                });

            var fromDate = new DateTime(2024, 5, 1);
            var toDate = new DateTime(2024, 5, 10);
            var result = await _repository.LoadBcPaymentDetailAsync("TEST_DB", fromDate: fromDate, toDate: toDate);

            Assert.Single(result);
            Assert.DoesNotContain("DELIVERYID = @DELIVERY_ID", capturedSql);
            Assert.Contains("CREATED_DATE >= @FROM_DATE AND CREATED_DATE < DATEADD(DAY, 1, @TO_DATE)", capturedSql);
            Assert.NotNull(capturedParams);
            Assert.Equal(fromDate.Date, capturedParams.Get<DateTime>("FROM_DATE"));
            Assert.Equal(toDate.Date, capturedParams.Get<DateTime>("TO_DATE"));
        }

        [Fact]
        public async Task LoadBcPaymentDetailAsync_WithMonthAndYear_ShouldIncludeMonthAndYearFilter()
        {
            string capturedSql = string.Empty;
            DynamicParameters? capturedParams = null;

            _mockSqlDataAccess
                .Setup(db => db.LoadData<BcPaymentDetailResponse, dynamic>(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
                .Callback<string, object, CommandType, string>((sql, param, cmdType, conn) =>
                {
                    capturedSql = sql;
                    capturedParams = param as DynamicParameters;
                })
                .ReturnsAsync(new List<BcPaymentDetailResponse>
                {
                    new() { Id = 3, DbCode = "TEST_DB", Total = 300 }
                });

            var result = await _repository.LoadBcPaymentDetailAsync("TEST_DB", deliveryId: "DEL02", month: 6, year: 2024);

            Assert.Single(result);
            Assert.Contains("DELIVERYID = @DELIVERY_ID", capturedSql);
            Assert.Contains("MONTH(CREATED_DATE) = @MONTH AND YEAR(CREATED_DATE) = @YEAR", capturedSql);
            Assert.NotNull(capturedParams);
            Assert.Equal(6, capturedParams.Get<int>("MONTH"));
            Assert.Equal(2024, capturedParams.Get<int>("YEAR"));
            Assert.Equal("DEL02", capturedParams.Get<string>("DELIVERY_ID"));
        }

        [Fact]
        public async Task LoadBcPaymentDetailAsync_WithPeriodInt_ShouldExtractMonthAndYear()
        {
            string capturedSql = string.Empty;
            DynamicParameters? capturedParams = null;

            _mockSqlDataAccess
                .Setup(db => db.LoadData<BcPaymentDetailResponse, dynamic>(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
                .Callback<string, object, CommandType, string>((sql, param, cmdType, conn) =>
                {
                    capturedSql = sql;
                    capturedParams = param as DynamicParameters;
                })
                .ReturnsAsync(new List<BcPaymentDetailResponse>
                {
                    new() { Id = 4, DbCode = "TEST_DB", Total = 400 }
                });

            var result = await _repository.LoadBcPaymentDetailAsync("TEST_DB", period: 202407);

            Assert.Single(result);
            Assert.Contains("MONTH(CREATED_DATE) = @MONTH AND YEAR(CREATED_DATE) = @YEAR", capturedSql);
            Assert.NotNull(capturedParams);
            Assert.Equal(7, capturedParams.Get<int>("MONTH"));
            Assert.Equal(2024, capturedParams.Get<int>("YEAR"));
        }

        [Fact]
        public async Task LoadBcPaymentDetailAsync_Overloads_ShouldDelegateCorrectly()
        {
            _mockSqlDataAccess
                .Setup(db => db.LoadData<BcPaymentDetailResponse, dynamic>(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
                .ReturnsAsync(new List<BcPaymentDetailResponse>
                {
                    new() { Id = 5, DbCode = "TEST_DB", Total = 500 }
                });

            var byDate = await _repository.LoadBcPaymentDetailAsync("TEST_DB", deliveryId: "DEL01", date: DateTime.Today);
            Assert.Single(byDate);

            var byDateRange = await _repository.LoadBcPaymentDetailAsync("TEST_DB", deliveryId: "DEL01", fromDate: DateTime.Today.AddDays(-5), toDate: DateTime.Today);
            Assert.Single(byDateRange);

            var byPeriod = await _repository.LoadBcPaymentDetailAsync("TEST_DB", deliveryId: "DEL01", month: 8, year: 2024);
            Assert.Single(byPeriod);

            var byPeriodNoDelivery = await _repository.LoadBcPaymentDetailAsync("TEST_DB", month: 8, year: 2024);
            Assert.Single(byPeriodNoDelivery);

            var byDateRangeNoDelivery = await _repository.LoadBcPaymentDetailAsync("TEST_DB", fromDate: DateTime.Today.AddDays(-5), toDate: DateTime.Today);
            Assert.Single(byDateRangeNoDelivery);

            var byDateNoDelivery = await _repository.LoadBcPaymentDetailAsync("TEST_DB", date: DateTime.Today);
            Assert.Single(byDateNoDelivery);
        }
    }
}
