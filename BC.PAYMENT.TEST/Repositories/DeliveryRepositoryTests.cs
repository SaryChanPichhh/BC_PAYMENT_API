using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using BC.PAYMENT.INFRASTRUCTURE.Repository.General;
using Moq;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace BC.PAYMENT.TEST.Repositories
{
    public class DeliveryRepositoryTests
    {
        private readonly Mock<ISqlDataAccess> _mockSqlDataAccess;
        private readonly DeliveryRepository _repository;

        public DeliveryRepositoryTests()
        {
            _mockSqlDataAccess = new Mock<ISqlDataAccess>();
            _repository = new DeliveryRepository(_mockSqlDataAccess.Object);
        }

        [Fact]
        public async Task CreateDelivery_ShouldReturnTrue_WhenRowsAffected()
        {
            // Arrange
            var delivery = new Delivery
            {
                DeliveryId = "DEL-001",
                DbCode = "TEST",
                DeliveryName = "Test"
            };

            _mockSqlDataAccess
                .Setup(db => db.ExecuteAsync(It.IsAny<string>(), It.IsAny<Delivery>(), CommandType.Text, "Default"))
                .ReturnsAsync(1);

            // Act
            var result = await _repository.CreateDelivery(delivery);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateDelivery_ShouldReturnTrue_WhenRowsAffected()
        {
            // Arrange
            var delivery = new Delivery
            {
                DeliveryId = "DEL-001",
                DbCode = "TEST",
                DeliveryName = "Updated"
            };

            _mockSqlDataAccess
                .Setup(db => db.ExecuteAsync(It.IsAny<string>(), It.IsAny<Delivery>(), CommandType.Text, "Default"))
                .ReturnsAsync(1);

            // Act
            var result = await _repository.UpdateDelivery(delivery);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteDelivery_ShouldReturnTrue_WhenRowsAffected()
        {
            // Arrange
            var id = "DEL-001";
            _mockSqlDataAccess
                .Setup(db => db.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
                .ReturnsAsync(1);

            // Act
            var result = await _repository.DeleteDelivery(id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetDeliveryByPermissionAsync_ShouldReturnList()
        {
            var mockData = new List<BC.PAYMENT.CORE.Contracts.Response.Delivery.DeliveryResponse>
            {
                new() { DeliveryId = "DEL-001", DeliveryNameKhmer = "Test Delivery" }
            };
            _mockSqlDataAccess
                .Setup(db => db.LoadData<BC.PAYMENT.CORE.Contracts.Response.Delivery.DeliveryResponse, dynamic>(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
                .ReturnsAsync(mockData);

            var result = await _repository.GetDeliveryByPermissionAsync("TEST_DB");

            Assert.Single(result);
            Assert.Equal("DEL-001", result[0].DeliveryId);
        }
    }
}
