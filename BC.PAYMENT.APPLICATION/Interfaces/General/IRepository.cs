namespace BC.PAYMENT.APPLICATION.Interfaces.General;

public interface IRepository<T> where T : class
{
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<T> GetByIdAsync(long id);
    Task<string> AddAsync(T entity);
    Task<string> UpdateAsync(T entity);
    Task<string> DeleteAsync(long id);
}