using BC.PAYMENT.API.Controllers.Submit;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.APPLICATION.Interfaces.Submit;
using BC.PAYMENT.CORE.Contracts.Request.Expense;
using BC.PAYMENT.CORE.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace BC.PAYMENT.TEST.Controllers
{
    public class SubmitExpenseControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ISubmitExpenseRepository> _mockSubmitExpenseRepo;
        private readonly SubmitExpenseController _controller;

        public SubmitExpenseControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockSubmitExpenseRepo = new Mock<ISubmitExpenseRepository>();

            _mockUnitOfWork.Setup(u => u.SubmitExpense).Returns(_mockSubmitExpenseRepo.Object);

            _controller = new SubmitExpenseController(_mockUnitOfWork.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("DbCode", "TEST_DB"),
                new Claim("Username", "TEST_USER"),
                new Claim("CurrentDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task AddNewSubmitExpenseAsync_ShouldReturnOk_WhenSuccessful()
        {
            var request = new CreateBcSubmittedPaidDetailRequest
            {
                PaidDetailId = 10,
                Dollar = 50,
                Riel = 200000,
                Exchange = 4000,
                Total = 100,
                ExpenseRiel = 10000,
                ExpenseDollar = 2.5,
                MoneyBias = 0,
                Status = true
            };

            BcSubmittedPaidDetail? capturedDetail = null;
            _mockSubmitExpenseRepo.Setup(r => r.AddNewSubmitExpense(It.IsAny<BcSubmittedPaidDetail>()))
                .Callback<BcSubmittedPaidDetail>(d => capturedDetail = d)
                .ReturnsAsync(1);

            var result = await _controller.AddNewSubmitExpenseAsync(request);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Success);
            Assert.Equal(1, result.Result);

            Assert.NotNull(capturedDetail);
            Assert.Equal("TEST_DB", capturedDetail!.DbCode);
            Assert.Equal("TEST_USER", capturedDetail.SubmittedBy);
            Assert.Equal(10, capturedDetail.PaidDetailId);
            Assert.Equal(50, capturedDetail.Dollar);
            Assert.Equal(200000, capturedDetail.Riel);
            Assert.Equal(4000, capturedDetail.Exchange);
            Assert.Equal(100, capturedDetail.Total);
            Assert.Equal(10000, capturedDetail.ExpenseRiel);
            Assert.Equal(2.5, capturedDetail.ExpenseDollar);
            Assert.True(capturedDetail.Status);
        }

        [Fact]
        public async Task AddNewSubmitExpenseAsync_ShouldReturnBadRequest_WhenInsertFails()
        {
            var request = new CreateBcSubmittedPaidDetailRequest
            {
                PaidDetailId = 10,
                Dollar = 50,
                Status = true
            };

            _mockSubmitExpenseRepo.Setup(r => r.AddNewSubmitExpense(It.IsAny<BcSubmittedPaidDetail>()))
                .ReturnsAsync(0);

            var result = await _controller.AddNewSubmitExpenseAsync(request);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task AddNewSubmitExpenseAsync_ShouldReturnException_WhenRepositoryThrows()
        {
            var request = new CreateBcSubmittedPaidDetailRequest
            {
                PaidDetailId = 10
            };

            _mockSubmitExpenseRepo.Setup(r => r.AddNewSubmitExpense(It.IsAny<BcSubmittedPaidDetail>()))
                .ThrowsAsync(new Exception("Database connection failed"));

            var result = await _controller.AddNewSubmitExpenseAsync(request);

            Assert.Equal((int)HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task UpdateSubmitExpenseDescriptionAsync_ShouldReturnOk_WhenSuccessful()
        {
            var request = new UpdateSubmitExpenseDescriptionRequest
            {
                SubmittedId = 1,
                DescExp1 = "Desc 1",
                DescExp2 = "Desc 2",
                DescExp3 = "Desc 3"
            };

            _mockSubmitExpenseRepo.Setup(r => r.UpdateSubmitExpenseDescriptionAsync(request))
                .ReturnsAsync(1);

            var result = await _controller.UpdateSubmitExpenseDescriptionAsync(request);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Success);
            Assert.Equal(1, result.Result);
        }

        [Fact]
        public async Task UpdateSubmitExpenseDescriptionAsync_ShouldReturnBadRequest_WhenUpdateFails()
        {
            var request = new UpdateSubmitExpenseDescriptionRequest
            {
                SubmittedId = 1
            };

            _mockSubmitExpenseRepo.Setup(r => r.UpdateSubmitExpenseDescriptionAsync(request))
                .ReturnsAsync(0);

            var result = await _controller.UpdateSubmitExpenseDescriptionAsync(request);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task DeleteSubmitExpenseAsync_ShouldReturnOk_WhenSuccessful()
        {
            _mockSubmitExpenseRepo.Setup(r => r.DeleteSubmitExpenseAsync(1))
                .ReturnsAsync(1);

            var result = await _controller.DeleteSubmitExpenseAsync(1);

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Success);
            Assert.Equal(1, result.Result);
        }

        [Fact]
        public async Task DeleteSubmitExpenseAsync_ShouldReturnBadRequest_WhenDeleteFails()
        {
            _mockSubmitExpenseRepo.Setup(r => r.DeleteSubmitExpenseAsync(1))
                .ReturnsAsync(0);

            var result = await _controller.DeleteSubmitExpenseAsync(1);

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.False(result.Success);
        }
    }
}
