using BC.PAYMENT.CORE.Contracts.Setting.Preset;
using BC.PAYMENT.CORE.Entities.Setting.Preset;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Prepare.Preset;
using Moq;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace BC.PAYMENT.TEST.Repositories.Prepare.Preset
{
    public class DistrictRepositoryTests
    {
        private readonly Mock<ISqlDataAccess> _mockSqlDataAccess;
        private readonly DistrictRepository _repository;

        public DistrictRepositoryTests()
        {
            _mockSqlDataAccess = new Mock<ISqlDataAccess>();
            _repository = new DistrictRepository(_mockSqlDataAccess.Object);
        }

        [Fact]
        public async Task AddNewAsync_ExecutesSuccessfully()
        {
            var model = new DistrictModel { DistrictId = 1, District = "Test District", ProvinceId = 1 };
            _mockSqlDataAccess
                .Setup(db => db.ExecuteAsync<object>(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
                .ReturnsAsync(1);

            var result = await _repository.AddNewAsync(model);

            Assert.True(result >= 0);
        }

        [Fact]
        public async Task UpdateAsync_ExecutesSuccessfully()
        {
            var model = new DistrictModel { DistrictId = 1, District = "Test District Updated", ProvinceId = 1 };
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
        public async Task GetAsync_ReturnsData()
        {
            var mockData = new List<DistrictResponseDTO> { new DistrictResponseDTO() };
            _mockSqlDataAccess
                .Setup(db => db.LoadData<DistrictResponseDTO, dynamic>(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
                .ReturnsAsync(mockData);

            var result = await _repository.GetAsync("DB01");

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetDistrictsByProvinceAsync_ReturnsData()
        {
            var mockData = new List<DistrictResponseDTO> { new DistrictResponseDTO() };
            _mockSqlDataAccess
                .Setup(db => db.LoadData<DistrictResponseDTO, dynamic>(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
                .ReturnsAsync(mockData);

            var result = await _repository.GetDistrictsByProvinceAsync("Prov1");

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetDistrictsByDistrictAsync_ReturnsData()
        {
            var mockData = new List<DistrictResponseDTO> { new DistrictResponseDTO() };
            _mockSqlDataAccess
                .Setup(db => db.LoadData<DistrictResponseDTO, dynamic>(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
                .ReturnsAsync(mockData);

            var result = await _repository.GetDistrictsByDistrictAsync("Dist1");

            Assert.NotNull(result);
            Assert.Single(result);
        }
    }
}
