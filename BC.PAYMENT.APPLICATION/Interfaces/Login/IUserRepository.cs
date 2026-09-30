using BC.PAYMENT.CORE.Contracts.Login;

namespace BC.PAYMENT.APPLICATION.Interfaces.Login
{
    public interface IUserRepository
    {
        public Task<IReadOnlyList<User>> GetAllAsync();
        public Task<User?> GetBcUserCredential(LoginRequestDTO requestDto);
        public Task<User> GetUserByIdAsync(ContextDTO contextDto);
        public Task<string> GetUserForOTP(string username);
        public Task<bool> IsExistsUserName(string username);
    }
}
