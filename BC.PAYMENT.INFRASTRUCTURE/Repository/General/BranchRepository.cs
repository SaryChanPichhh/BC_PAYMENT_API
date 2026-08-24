namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General
{
    public class BranchRepository : IBranchRepository
    {
        #region ===[ Private Members ]=============================================================

        private readonly ISqlDataAccess _sqlDataAccess;

        #endregion

        #region ===[ Constructor ]=================================================================

        public BranchRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        #endregion

        #region ===[ IUserRepository Methods ]==================================================

        public async Task<List<BranchDTO>> GetLoginBranchAsync(string username, string appCode = "PYS")
        {
            return (await _sqlDataAccess.LoadData<BranchDTO, dynamic>(BranchQueries.LoginBranches, new { USER_NAME = username, APP_CODE = appCode })).ToList();
        }

        public async Task<List<BranchDTO>> GetBranchAsync()
        {
            return (await _sqlDataAccess.LoadData<BranchDTO, dynamic>(BranchQueries.AllBranches, new { })).ToList();
        }
        #endregion

    }
}
