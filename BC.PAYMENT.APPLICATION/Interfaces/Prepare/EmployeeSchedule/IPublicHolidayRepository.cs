using BC.PAYMENT.APPLICATION.Interfaces.BaseInterface;
using BC.PAYMENT.CORE.Entities.Prepare.EmployeeSchedule;
namespace BC.PAYMENT.APPLICATION.Interfaces.Prepare.EmployeeSchedule
{
    public interface IPublicHolidayRepository : IBaseRepository<PublicHolidayModel>
    {
        Task<string> GetMaxCodePublicHolidayAsync();
        Task<List<PublicHolidayModel>> GetListHoliday();
    }
}
