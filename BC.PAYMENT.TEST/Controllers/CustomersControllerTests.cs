using BC.PAYMENT.API.Controllers;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Contracts.Response.Customer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using BC.PAYMENT.API.Helper;
using Xunit;

namespace BC.PAYMENT.TEST.Controllers
{
    public class CustomersControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICustomerRepository> _mockRepo;
        private readonly Mock<IOptions<AppSettings>> _mockOptions;
        private readonly CustomersController _controller;

        public CustomersControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<ICustomerRepository>();
            _mockOptions = new Mock<IOptions<AppSettings>>();

            _mockUnitOfWork.Setup(u => u.Customers).Returns(_mockRepo.Object);

            _controller = new CustomersController(_mockUnitOfWork.Object, _mockOptions.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity([
                new Claim("DbCode", "TEST_DB"),
                new Claim("Username", "TEST_USER")
            ], "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetCustomer_ShouldReturnOk_WhenCustomersExist()
        {
            var mockCustomers = new List<Customer>
            {
                new Customer { CustomerCode = "C001", CustomerName = "John Doe" },
                new Customer { CustomerCode = "C002", CustomerName = "Jane Doe" }
            };
            
            _mockRepo.Setup(r => r.GetCustomer(1, 10)).ReturnsAsync(mockCustomers);

            var result = await _controller.GetCustomer(1, 10);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(2, result.Result.TotalRecords);
            Assert.Equal(2, result.Result.Data.Count);
        }
        
        [Fact]
        public async Task GetCustomer_ShouldReturnBadRequest_WhenNoCustomers()
        {
            _mockRepo.Setup(r => r.GetCustomer(1, 10)).ReturnsAsync(new List<Customer>());

            var result = await _controller.GetCustomer(1, 10);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Empty(result.Result.Data);
        }

        [Fact]
        public async Task GetCustomerInfoByMarketId_ShouldReturnOk_WhenCustomersExist()
        {
            var mockData = new List<CustomerResponse>
            {
                new CustomerResponse { CustomerCode = "C001", CustomerName = "John Doe" }
            };
            
            _mockRepo.Setup(r => r.GetCustomerInfoByMarketIdAsync("M01")).ReturnsAsync(mockData);

            var result = await _controller.GetCustomerInfoByMarketId("M01");

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Single(result.Result);
            Assert.Equal("C001", result.Result[0].CustomerCode);
        }
    }
}
