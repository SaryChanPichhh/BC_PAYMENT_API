using System.Data;

namespace BC.PAYMENT.INFRASTRUCTURE.DBAccess
{
    public interface ISqlDataAccess
    {
        /// <summary>
        /// This method uses for execute all command such as Insert,UpdateAsync,DisableAsync
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="storedProcedure"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        Task<int> ExecuteAsync<U>(string storedProcedure, U parameters, CommandType commandType = CommandType.Text,
            string connectionString = "Default");

        /// <summary>
        /// This method users for load list data from server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="storedProcedure"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> LoadData<T, U>(string storedProcedure, U parameters,
            CommandType commandType = CommandType.Text, string connectionString = "Default");

        /// <summary>
        /// This method users for load single data from server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TU"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        Task<T> LoadSingleData<T, TU>(string query, TU parameters, CommandType commandType = CommandType.Text,
            string connectionString = "Default");

        // Dapper execute scalar
        Task<T> ExecuteScalarAsync<T, TU>(string query, TU parameters, CommandType commandType = CommandType.Text,
            string connectionString = "Default");

        T ExecuteScalar<T, TU>(string query, TU parameters, CommandType commandType = CommandType.Text,
            string connectionString = "Default");

        Task<(IEnumerable<T1>, IEnumerable<T2>)> LoadMultipleData<T1, T2, TU>(
            string query,
            TU parameters,
            CommandType commandType = CommandType.Text,
            string connectionString = "Default");
    }
}
