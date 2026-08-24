namespace BC.PAYMENT.APPLICATION.Interfaces.General
{
    public interface IEmployeeRepository
    {
        Task<List<EmployeeModel>> GetAllHasCompletedPayment();
        Task<List<TemplateModel>> GetAllTemplateByEmployeId(int employeeId);
    }
}
