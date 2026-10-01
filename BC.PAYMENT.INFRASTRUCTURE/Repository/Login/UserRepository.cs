using BC.PAYMENT.CORE.Contracts.Login;
using Microsoft.Extensions.Configuration;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Login;

public class UserRepository : IUserRepository
{
    #region SuperAdminCredential Members

    private readonly string SuperAdmin;
    private readonly string SuperAdminPassword;

    #endregion


    #region ===[ Private Members ]=============================================================

    private readonly ISqlDataAccess _sqlDataAccess;
    private readonly IInvoiceClosingEntryRepository _invoiceClosingEntryRepository;

    #endregion

    #region ===[ Constructor ]=================================================================

    public UserRepository(ISqlDataAccess sqlDataAccess, IInvoiceClosingEntryRepository invoiceClosingEntryRepository)
    {
        _sqlDataAccess = sqlDataAccess;
        _invoiceClosingEntryRepository = invoiceClosingEntryRepository;

        SuperAdmin = Singleton.Instance.Username ?? "BCSA";
        SuperAdminPassword = Singleton.Instance.UserPassword ?? "Bc@dmin";
    }

    #endregion

    #region ===[ IUserRepository Methods ]==================================================

    public async Task<IReadOnlyList<User>> GetAllAsync()
    {
        var result = await _sqlDataAccess.LoadData<User, dynamic>(UserStoreProcedures.AllUser, new { });
        return result.ToList();
    }

    public async Task<User?> GetBcUserCredential(LoginRequestDTO requestDto)
    {
        try
        {
            var param = new
            {
                USER_NAME = requestDto.Username,
                APP_CODE = requestDto.AppCode,
                DB_CODE = requestDto.DbCode,
                COMPANYCODE = requestDto.CompanyCode
            };
            User user;
            if (requestDto.Username.Equals(SuperAdmin) && requestDto.Password.Equals(SuperAdminPassword))
            {
                user = new User
                {
                    Username = requestDto.Username,
                    DbCode = requestDto.DbCode,
                    Role = "ADMIN",
                    CompanyCode = requestDto.CompanyCode,
                    CurrentDate = DateTime.Now,
                    Name = requestDto.Username, AppCode = requestDto.AppCode
                };
            }
            else
            {
                user = await _sqlDataAccess.LoadSingleData<User, dynamic>(UserStoreProcedures.GetCredential, param);
                if (user is null)
                    return user;
                user.Role = "USER";
            }

            if (requestDto.AppCode == "PYS")
            {
                var entrycode =
                    await _invoiceClosingEntryRepository.CheckIsEntriesIsAlreadyOpenAsync(requestDto.DbCode);
                if (!entrycode)
                {
                    var generateOpeningEntryCodeAsync =
                        await _invoiceClosingEntryRepository.GenerateOpeningEntryCodeAsync(requestDto.DbCode);
                    await _invoiceClosingEntryRepository.CreateClosingEntryAsync(new InvoiceClosingEntriesModel
                    {
                        DbCode = requestDto.DbCode,
                        IsActive = true,
                        CreatedBy = "System",
                        CreatedAt = DateTime.Now,
                        Code = generateOpeningEntryCodeAsync,
                        Description = "Auto Generate Opening Entry By System"
                    });
                }

                var openingEntryCodeByDbCodeAsync =
                    await _invoiceClosingEntryRepository.GetOpeningEntryCodeByDbCodeAsync(requestDto.DbCode);
                user.InvoiceEntryCode = openingEntryCodeByDbCodeAsync;
                user.AppCode = requestDto.AppCode;
            }

            return user;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<User> GetUserByIdAsync(ContextDTO contextDto)
    {
        if (contextDto.UserId == 0)
            return new User
            {
                UserId = 0,
                Username = SuperAdmin,
                DbCode = contextDto.DbCode,
                Role = "ADMIN",
                AppCode = contextDto.AppCode
            };

        var param = new
        {
            USER_ID = contextDto.UserId,
            APP_CODE = contextDto.AppCode,
            DB_CODE = contextDto.DbCode
        };
        return await _sqlDataAccess.LoadSingleData<User, dynamic>(UserStoreProcedures.UserById, param);
    }

    public async Task<string> GetUserForOTP(string username)
    {
        var sql =
            @"SELECT USER_ID FROM BCUSERS WHERE USER_NAME = @USER_NAME AND USER_STATUS = '1'";
        var param = new
        {
            USER_NAME = username
        };
        return await _sqlDataAccess.LoadSingleData<string, dynamic>(sql, param);
    }

    public async Task<bool> IsExistsUserName(string username)
    {
        var execute = await _sqlDataAccess.LoadSingleData<bool, dynamic>(UserStoreProcedures.IsExistsUser
            , new { APP_CODE = "PYS", USER_NAME = username });
        return execute;
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