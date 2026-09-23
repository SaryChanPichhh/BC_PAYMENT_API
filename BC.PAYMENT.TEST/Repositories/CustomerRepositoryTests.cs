using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Contracts.Response.Customer;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using BC.PAYMENT.INFRASTRUCTURE.Repository.General;
using Moq;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace BC.PAYMENT.TEST.Repositories
{
    public class CustomerRepositoryTests
    {
        private readonly Mock<ISqlDataAccess> _mockSqlDataAccess;
        private readonly CustomerRepository _repository;

        public CustomerRepositoryTests()
        {
            _mockSqlDataAccess = new Mock<ISqlDataAccess>();
            _repository = new CustomerRepository(_mockSqlDataAccess.Object);
        }

        [Fact]
        public async Task GetCustomer_WithPagination_ShouldReturnList()
        {
            var mockData = new List<Customer> { new Customer { CustomerCode = "C01" } };
            _mockSqlDataAccess.Setup(db => db.LoadData<Customer, dynamic>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CommandType>(), "Default"))
                              .ReturnsAsync(mockData);

            var result = await _repository.GetCustomer(1, 10);

            Assert.Single(result);
            Assert.Equal("C01", result[0].CustomerCode);
        }

        [Fact]
        public async Task GetCustomer_ShouldReturnList()
        {
            var mockData = new List<Customer> { new Customer { CustomerCode = "C01" } };
            _mockSqlDataAccess.Setup(db => db.LoadData<Customer, dynamic>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CommandType>(), "Default"))
                              .ReturnsAsync(mockData);

            var result = await _repository.GetCustomer();

            Assert.Single(result);
        }

        [Fact]
        public async Task GetCustomerByMarketCodeAsync_ShouldReturnList()
        {
            var mockData = new List<Customer> { new Customer { CustomerCode = "C01" } };
            _mockSqlDataAccess.Setup(db => db.LoadData<Customer, dynamic>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CommandType>(), "Default"))
                              .ReturnsAsync(mockData);

            var result = await _repository.GetCustomerByMarketCodeAsync("DB01", new List<string> { "M01" }, new List<string> { "S01" });

            Assert.Single(result);
        }

        [Fact]
        public async Task GetCustomerInfoByMarketIdAsync_ShouldReturnResponseList()
        {
            var mockData = new List<CustomerResponse> { new CustomerResponse { CustomerCode = "C01" } };
            _mockSqlDataAccess.Setup(db => db.LoadData<CustomerResponse, dynamic>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CommandType>(), "Default"))
                              .ReturnsAsync(mockData);

            var result = await _repository.GetCustomerInfoByMarketIdAsync("M01");

            Assert.Single(result);
            Assert.Equal("C01", result[0].CustomerCode);
        }

        [Fact]
        public async Task GetCustomerWhoWrongAreaAndMarketAsync_ShouldReturnResponseList()
        {
            var mockData = new List<CustomerResponse> 
            { 
                new CustomerResponse 
                { 
                    CustomerCode = "C003", 
                    Area = "A01", 
                    AreaOS = "A02" 
                } 
            };
            
            _mockSqlDataAccess.Setup(db => db.LoadData<CustomerResponse, dynamic>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CommandType>(), "Default"))
                              .ReturnsAsync(mockData);

            var result = await _repository.GetCustomerWhoWrongAreaAndMarketAsync("DB01");

            Assert.Single(result);
            Assert.Equal("C003", result[0].CustomerCode);
            Assert.Equal("A01", result[0].Area);
            Assert.Equal("A02", result[0].AreaOS);
        }

        [Fact]
        public async Task GetAllCustomerInfoAsync_ShouldReturnList()
        {
            var mockData = new List<CustomerResponse> { new CustomerResponse { CustomerCode = "C01" } };
            _mockSqlDataAccess.Setup(db => db.LoadData<CustomerResponse, dynamic>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CommandType>(), "Default"))
                              .ReturnsAsync(mockData);

            var result = await _repository.GetAllCustomerInfoAsync("DB01", 1, 10);

            Assert.Single(result);
            Assert.Equal("C01", result[0].CustomerCode);
        }

        [Fact]
        public async Task GetAllCustomerInfoCountAsync_ShouldReturnInt()
        {
            _mockSqlDataAccess.Setup(db => db.LoadSingleData<int, dynamic>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CommandType>(), "Default"))
                              .ReturnsAsync(5);

            var result = await _repository.GetAllCustomerInfoCountAsync("DB01");

            Assert.Equal(5, result);
        }

        [Fact]
        public async Task GetCustomerImageAsync_ShouldReturnByteArray()
        {
            var mockBytes = new byte[] { 0x01, 0x02 };
            _mockSqlDataAccess.Setup(db => db.LoadSingleData<byte[], dynamic>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CommandType>(), "Default"))
                              .ReturnsAsync(mockBytes);

            var result = await _repository.GetCustomerImageAsync("DB01", "C01");

            Assert.Equal(mockBytes, result);
        }
    }
}
