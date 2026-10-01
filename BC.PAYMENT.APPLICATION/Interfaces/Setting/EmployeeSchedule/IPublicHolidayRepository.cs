namespace BC.PAYMENT.APPLICATION.Interfaces.Prepare.EmployeeSchedule;

public interface IPublicHolidayRepository : IBaseRepository<PublicHolidayModel>
{
    Task<string> GetMaxCodePublicHolidayAsync();
    Task<List<PublicHolidayModel>> GetListHoliday();
}