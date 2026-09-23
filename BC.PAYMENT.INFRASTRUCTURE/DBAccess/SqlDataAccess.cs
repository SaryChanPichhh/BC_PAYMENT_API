namespace BC.PAYMENT.INFRASTRUCTURE.DBAccess
{
    public class SqlDataAccess(IConfiguration settings) : ISqlDataAccess
    {
        public async Task<int> ExecuteAsync<U>(string storedProcedure, U parameters, CommandType commandType = CommandType.Text, string connectionString = "Default")
        {
            if (connectionString == "Default")
            {
                connectionString = settings.GetConnectionString("DBConnection")!;
            }

            await using var connection = new SqlConnection(connectionString);
            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync();


            var transaction = connection.BeginTransaction();
            try 
            {
                var result = await connection.ExecuteAsync(storedProcedure, parameters, transaction: transaction, commandType: commandType);
                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public T ExecuteScalar<T, TU>(string query, TU parameters, CommandType commandType = CommandType.Text, string connectionString = "Default")
        {
            if (connectionString == "Default")
            {
                connectionString = settings.GetConnectionString("DBConnection")!;
            }
            using var connection = new SqlConnection(connectionString);
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            return connection.ExecuteScalar<T>(query, parameters, commandType: commandType);
        }

        public async Task<T> ExecuteScalarAsync<T, TU>(string query, TU parameters, CommandType commandType = CommandType.Text, string connectionString = "Default")
        {
            {
                if (connectionString == "Default")
                {
                    connectionString = settings.GetConnectionString("DBConnection")!;
                }

                await using var connection = new SqlConnection(connectionString);
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();
                return await connection.ExecuteScalarAsync<T>(query, parameters, commandType: commandType);
            }
        }
        public async Task<IEnumerable<T>> LoadData<T, U>(string storedProcedure, U parameters, CommandType commandType = CommandType.Text, string connectionString = "Default")
        {
            if (connectionString == "Default")
            {
                connectionString = settings.GetConnectionString("DBConnection")!;
            }

            await using var connection = new SqlConnection(connectionString);
            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync();
            return await connection.QueryAsync<T>(storedProcedure, parameters, commandType: commandType);
        }

        public async Task<T> LoadSingleData<T, TU>(string query, TU parameters, CommandType commandType = CommandType.Text, string connectionString = "Default")
        {
            if (connectionString == "Default")
            {
                connectionString = settings.GetConnectionString("DBConnection")!;
            }

            using var connection = new SqlConnection(connectionString);
            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync();
            return await connection.QueryFirstOrDefaultAsync<T>(query, parameters, commandType: commandType);
        }

        public async Task<(IEnumerable<T1>, IEnumerable<T2>)> LoadMultipleData<T1, T2, TU>(
            string query,
            TU parameters,
            CommandType commandType = CommandType.Text,
            string connectionString = "Default")
        {
            if (connectionString == "Default")
            {
                connectionString = settings.GetConnectionString("DBConnection")!;
            }

            using var connection = new SqlConnection(connectionString);
            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync();

            // Execute QueryMultipleAsync
            using var multi = await connection.QueryMultipleAsync(query, parameters, commandType: commandType);

            // Read first and second result sets
            var resultSet1 = await multi.ReadAsync<T1>();
            var resultSet2 = await multi.ReadAsync<T2>();

            return (resultSet1, resultSet2);
        }

    }
}
