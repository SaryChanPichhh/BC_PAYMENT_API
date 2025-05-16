using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.APPLICATION.Interfaces.Login;
using BC.PAYMENT.CORE.DTO.General;
using BC.PAYMENT.CORE.DTO.Login;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Entities.Login;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using BC.PAYMENT.SQL.Queries;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Login
{
    public class UserRepository : IUserRepository
    {

        #region ===[ Private Members ]=============================================================
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly IInvoiceClosingEntryRepository _invoiceClosingEntryRepository;

        #endregion

        #region ===[ Constructor ]=================================================================

        public UserRepository(ISqlDataAccess sqlDataAccess,IInvoiceClosingEntryRepository invoiceClosingEntryRepository)
        {
            _sqlDataAccess = sqlDataAccess;
            _invoiceClosingEntryRepository = invoiceClosingEntryRepository;
        }

        #endregion

        #region ===[ IUserRepository Methods ]==================================================

        public async Task<IReadOnlyList<User>> GetAllAsync()
        {
            var result = await _sqlDataAccess.LoadData<User,dynamic>(UserStoreProcedures.AllUser,new {});
            return result.ToList();
        }

        public async Task<User> GetBcUserCredential(LoginRequestDTO requestDto)
        {
            try
            {
                var param = new
                {
                    USER_NAME = requestDto.Username,
                    APP_CODE = requestDto.AppCode,
                    DB_CODE = requestDto.DbCode,
                    COMPANYCODE = requestDto.CompanyCode,
                };
                var result = await _sqlDataAccess.LoadSingleData<User, dynamic>(UserStoreProcedures.GetCredential,param);
                if (requestDto.AppCode == "PYS")
                {
                    var entrycode = await _invoiceClosingEntryRepository.CheckIsEntriesIsAlreadyOpenAsync(requestDto.DbCode);
                    if (!entrycode)
                    {
                        var generateOpeningEntryCodeAsync = await _invoiceClosingEntryRepository.GenerateOpeningEntryCodeAsync(requestDto.DbCode);
                        await _invoiceClosingEntryRepository.CreateClosingEntryAsync(new InvoiceClosingEntriesModel()
                        {
                            IsActive = true,
                            CreatedBy = "System",
                            CreatedDate = DateTime.Now,
                            Code = generateOpeningEntryCodeAsync,
                            Description = "Auto Generate Opening Entry By System",
                        }, requestDto.DbCode);
                    }
                    var openingEntryCodeByDbCodeAsync = await _invoiceClosingEntryRepository.GetOpeningEntryCodeByDbCodeAsync(requestDto.DbCode);
                    result.InvoiceEntryCode = openingEntryCodeByDbCodeAsync;
                }


                return result;

            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                throw;
            }

            
        }
        public async Task<User> GetUserByIdAsync(ContextDTO contextDto)
        {
            var param = new
            {
                USER_ID = contextDto.UserId,
                APP_CODE = contextDto.AppCode,
                DB_CODE = contextDto.DbCode,
            };
            return (await _sqlDataAccess.LoadSingleData<User, dynamic>(UserStoreProcedures.UserById, param));

        }

        //public async Task<List<User>> GetBcUserCredential(string username)
        //{
        //    using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString("DBConnection")))
        //    {
        //        using (IDbConnection connection = new SqlConnection(configuration.GetConnectionString("DBConnection")))
        //        {
        //            try
        //            {
        //                Define DynamicParameters to handle input parameters
        //                var parameters = new DynamicParameters();
        //                parameters.Add("@APP_CODE", requestDto.AppCode, DbType.String, ParameterDirection.Input);
        //                parameters.Add("@USER_NAME", requestDto.Username, DbType.String, ParameterDirection.Input);

        //                Output parameters
        //                parameters.Add("@USER_ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
        //                parameters.Add("@USER_PASS", dbType: DbType.String, direction: ParameterDirection.Output, size: -1);  // -1 for NVARCHAR(MAX)

        //                Query for users and their branches using multi-mapping
        //                var userDictionary = new Dictionary<int, LoginResponseDTO>();

        //                var result = await connection.QueryAsync<LoginResponseDTO, BranchDTO, LoginResponseDTO>(
        //                    UserStoreProcedures.IsUserAuthorized,   // Stored procedure name
        //                    (user, branch) =>
        //                    {
        //                        Check if the user already exists in the dictionary
        //                        if (!userDictionary.TryGetValue(user.UserId, out var currentUser))
        //                        {
        //                            currentUser = user;
        //                            currentUser.DbCodes = new List<BranchDTO>();  // Initialize Branches list
        //                            userDictionary.Add(currentUser.UserId, currentUser);
        //                        }

        //                        Add the branch to the user's list of branches
        //                        currentUser.DbCodes.Add(branch);
        //                        return currentUser;
        //                    },
        //                    parameters,
        //                    commandType: CommandType.StoredProcedure,
        //                    splitOn: "DbCode"
        //                );
        //                Retrieve output parameters after the query execution
        //               var userId = parameters.Get<int>("@USER_ID");
        //                var userPass = parameters.Get<string>("@USER_PASS");

        //                Optionally, log or use the output parameters
        //                Console.WriteLine($"UserId: {userId}, UserPassword: {userPass}");
        //                Return the users as a list
        //                return userDictionary.Values.ToList();
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine(ex);
        //                return new List<LoginResponseDTO>();
        //            }
        //        }
        //    }
        //}




        #endregion
    }
}

