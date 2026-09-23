using BC.PAYMENT.CORE.Entities;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using BC.PAYMENT.INFRASTRUCTURE.Repository.Submit;
using Moq;
using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace BC.PAYMENT.TEST.Repositories
{
    public class SubmitExpenseRepositoryTests
    {
        private readonly Mock<ISqlDataAccess> _mockSqlDataAccess;
        private readonly SubmitExpenseRepository _repository;

        public SubmitExpenseRepositoryTests()
        {
            _mockSqlDataAccess = new Mock<ISqlDataAccess>();
            _repository = new SubmitExpenseRepository(_mockSqlDataAccess.Object);
        }

        [Fact]
        public async Task AddNewSubmitExpense_ShouldReturnAffectedRows_WhenExecuted()
        {
            var detail = new BcSubmittedPaidDetail
            {
                PaidDetailId = 1,
                Dollar = 100,
                Riel = 400000,
                Exchange = 4000,
                Total = 200,
                ExpenseRiel = 20000,
                ExpenseDollar = 5,
                MoneyBais = 0,
                Status = true,
                DbCode = "TEST_DB",
                SubmittedDate = DateTime.Now,
                SubmittedBy = "TEST_USER"
            };

            _mockSqlDataAccess.Setup(db => db.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
                .ReturnsAsync(1);

            var result = await _repository.AddNewSubmitExpense(detail);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task UpdateSubmitExpenseDescriptionAsync_ShouldReturnAffectedRows_WhenExecuted()
        {
            var request = new BC.PAYMENT.CORE.Contracts.Request.Expense.UpdateSubmitExpenseDescriptionRequest
            {
                SubmittedId = 1,
                DescExp1 = "Desc 1",
                DescExp2 = "Desc 2",
                DescExp3 = "Desc 3"
            };

            _mockSqlDataAccess.Setup(db => db.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
                .ReturnsAsync(1);

            var result = await _repository.UpdateSubmitExpenseDescriptionAsync(request);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task DeleteSubmitExpenseAsync_ShouldReturnAffectedRows_WhenExecuted()
        {
            _mockSqlDataAccess.Setup(db => db.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
                .ReturnsAsync(1);

            var result = await _repository.DeleteSubmitExpenseAsync(1);

            Assert.Equal(1, result);
        }
    }
}
