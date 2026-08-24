namespace BC.PAYMENT.APPLICATION.Interfaces.General
{
    public interface IAreaRepository
    {
        Task<List<Area>> GetArea(string dbCode);
    }
}
