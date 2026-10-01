using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Contracts.Response.Area;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using BC.PAYMENT.INFRASTRUCTURE.Repository.General;
using Moq;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace BC.PAYMENT.TEST.Repositories;

public class AreaRepositoryTests
{
    private readonly Mock<ISqlDataAccess> _mockSqlDataAccess;
    private readonly AreaRepository _repository;

    public AreaRepositoryTests()
    {
        _mockSqlDataAccess = new Mock<ISqlDataAccess>();
        _repository = new AreaRepository(_mockSqlDataAccess.Object);
    }

    [Fact]
    public async Task GetArea_ShouldReturnList()
    {
        var mockData = new List<AreaResponse> { new() { AreaId = "A01", AreaName = "North" } };
        _mockSqlDataAccess.Setup(db =>
                db.LoadData<AreaResponse, dynamic>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CommandType>(),
                    "Default"))
            .ReturnsAsync(mockData);

        var result = await _repository.GetArea("DB01");

        Assert.Single(result);
        Assert.Equal("A01", result[0].AreaId);
    }

    [Fact]
    public async Task CreateAreaAsync_ShouldReturnTrue_WhenRowsAffected()
    {
        var entity = new Area { AreaId = "A01", DbCode = "DB01", AreaName = "North" };
        _mockSqlDataAccess.Setup(db =>
                db.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
            .ReturnsAsync(1);

        var result = await _repository.CreateAreaAsync(entity);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAreaAsync_ShouldReturnTrue_WhenRowsAffected()
    {
        var entity = new Area { AreaId = "A01", DbCode = "DB01", AreaName = "North Upd" };
        _mockSqlDataAccess.Setup(db =>
                db.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
            .ReturnsAsync(1);

        var result = await _repository.UpdateAreaAsync(entity);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAreaAsync_ShouldReturnTrue_WhenRowsAffected()
    {
        _mockSqlDataAccess.Setup(db =>
                db.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
            .ReturnsAsync(1);

        var result = await _repository.DeleteAreaAsync("A01", "DB01");

        Assert.True(result);
    }

    [Fact]
    public async Task GenerateAreaIdAsync_ShouldReturnString()
    {
        _mockSqlDataAccess.Setup(db =>
                db.LoadSingleData<string, dynamic>(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
            .ReturnsAsync("AREA-123");

        var result = await _repository.GenerateAreaIdAsync();

        Assert.Equal("AREA-123", result);
    }
}