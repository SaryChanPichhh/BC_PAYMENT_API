namespace BC.PAYMENT.APPLICATION.Interfaces.General
{
    public interface IBranchRepository
    {
        public Task<List<BranchDTO>> GetLoginBranchAsync(string username, string appCode = "PYS");
        public Task<List<BranchDTO>> GetBranchAsync();
    }
}
