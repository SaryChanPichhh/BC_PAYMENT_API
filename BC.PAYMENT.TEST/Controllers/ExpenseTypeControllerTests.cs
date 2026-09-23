using BC.PAYMENT.API.Controllers;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.API.Models.ExpenseTypes;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace BC.PAYMENT.TEST.Controllers
{
    public class ExpenseTypeControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IExpenseTypeRepository> _mockRepo;
        private readonly ExpenseTypeController _controller;

        public ExpenseTypeControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IExpenseTypeRepository>();

            _mockUnitOfWork.Setup(u => u.ExpenseTypes).Returns(_mockRepo.Object);

            _controller = new ExpenseTypeController(_mockUnitOfWork.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("DbCode", "TEST_DB"),
                new Claim("Username", "TEST_USER")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task CreateExpenseType_ShouldReturnCreated_WhenSuccessful()
        {
            var request = new CreateExpenseTypeRequest { ExpenseName = "Fuel" };
            _mockRepo.Setup(r => r.CreateExpenseType(It.IsAny<ExpenseType>())).ReturnsAsync(true);

            var result = await _controller.CreateExpenseType(request);

            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
            Assert.Equal("Fuel", result.Result.ExpenseName);
        }

        [Fact]
        public async Task UpdateExpenseType_ShouldReturnOk_WhenSuccessful()
        {
            var request = new UpdateExpenseTypeRequest { ExpenseName = "Updated" };
            _mockRepo.Setup(r => r.UpdateExpenseType(It.IsAny<ExpenseType>())).ReturnsAsync(true);

            var result = await _controller.UpdateExpenseType("E01", request);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Updated", result.Result.ExpenseName);
        }

        [Fact]
        public async Task DeleteExpenseType_ShouldReturnOk_WhenSuccessful()
        {
            _mockRepo.Setup(r => r.DeleteExpenseType("E01")).ReturnsAsync(true);

            var result = await _controller.DeleteExpenseType("E01");

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Result);
        }
    }
}
