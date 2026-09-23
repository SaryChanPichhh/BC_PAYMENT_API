using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using BC.PAYMENT.INFRASTRUCTURE.Repository.General;
using Moq;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace BC.PAYMENT.TEST.Repositories
{
    public class ExpenseTypeRepositoryTests
    {
        private readonly Mock<ISqlDataAccess> _mockSqlDataAccess;
        private readonly ExpenseTypeRepository _repository;

        public ExpenseTypeRepositoryTests()
        {
            _mockSqlDataAccess = new Mock<ISqlDataAccess>();
            _repository = new ExpenseTypeRepository(_mockSqlDataAccess.Object);
        }

        [Fact]
        public async Task CreateExpenseType_ShouldReturnTrue_WhenRowsAffected()
        {
            var entity = new ExpenseType { ExpenseId = "E01", DbCode = "DB01", ExpenseName = "Fuel" };
            _mockSqlDataAccess.Setup(db => db.ExecuteAsync(It.IsAny<string>(), It.IsAny<ExpenseType>(), CommandType.Text, "Default"))
                              .ReturnsAsync(1);

            var result = await _repository.CreateExpenseType(entity);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateExpenseType_ShouldReturnTrue_WhenRowsAffected()
        {
            var entity = new ExpenseType { ExpenseId = "E01", DbCode = "DB01", ExpenseName = "Fuel Upd" };
            _mockSqlDataAccess.Setup(db => db.ExecuteAsync(It.IsAny<string>(), It.IsAny<ExpenseType>(), CommandType.Text, "Default"))
                              .ReturnsAsync(1);

            var result = await _repository.UpdateExpenseType(entity);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteExpenseType_ShouldReturnTrue_WhenRowsAffected()
        {
            _mockSqlDataAccess.Setup(db => db.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), CommandType.Text, "Default"))
                              .ReturnsAsync(1);

            var result = await _repository.DeleteExpenseType("E01");

            Assert.True(result);
        }
    }
}
