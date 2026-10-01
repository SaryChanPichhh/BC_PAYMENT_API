using BC.PAYMENT.CORE.Contracts.Response.Province;
using BC.PAYMENT.CORE.Entities.Setting.Preset;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Prepare.Preset;
using Moq;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace BC.PAYMENT.TEST.Repositories.Prepare.Preset;

public class ProvinceRepositoryTests
{
    private readonly Mock<ISqlDataAccess> _mockSqlDataAccess;
    private readonly ProvinceRepository _repository;

    public ProvinceRepositoryTests()
    {
        _mockSqlDataAccess = new Mock<ISqlDataAccess>();
        _repository = new ProvinceRepository(_mockSqlDataAccess.Object);
    }

    [Fact]
    public async Task AddNewAsync_ExecutesSuccessfully()
    {
        var model = new ProvinceModel { Province = "Test Province" };
        _mockSqlDataAccess
            .Setup(db => db.ExecuteAsync<object>(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
            .ReturnsAsync(1);

        var result = await _repository.AddNewAsync(model);

        Assert.True(result >= 0);
    }

    [Fact]
    public async Task UpdateAsync_ExecutesSuccessfully()
    {
        var model = new ProvinceModel { ProvinceId = 1, Province = "Test Province Updated" };
        _mockSqlDataAccess
            .Setup(db => db.ExecuteAsync<object>(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
            .ReturnsAsync(1);

        var result = await _repository.UpdateAsync(model);

        Assert.True(result >= 0);
    }

    [Fact]
    public async Task DeleteAsync_ExecutesSuccessfully()
    {
        _mockSqlDataAccess
            .Setup(db => db.ExecuteAsync<object>(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
            .ReturnsAsync(1);

        var result = await _repository.DeleteAsync("1");

        Assert.True(result >= 0);
    }

    [Fact]
    public async Task GetAllProvinces_ReturnsData()
    {
        var mockData = new List<ProvinceResponse> { new() };
        _mockSqlDataAccess
            .Setup(db =>
                db.LoadData<ProvinceResponse, dynamic>(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text,
                    "Default"))
            .ReturnsAsync(mockData);

        var result = await _repository.GetAllProvinces();

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetAsync_ReturnsEmptyList()
    {
        var result = await _repository.GetAsync("DB01");

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}