using BC.PAYMENT.CORE.DTO;
using BC.PAYMENT.CORE.DTO.General;
using BC.PAYMENT.CORE.DTO.Login;
using BC.PAYMENT.CORE.Entities.Login;

namespace BC.PAYMENT.APPLICATION.Interfaces.Login
{
    public interface IUserRepository
    {
        public Task<User> GetBcUserCredential(LoginRequestDTO requestDto);
        public Task<User> GetUserByIdAsync(ContextDTO contextDto);

    }
}
