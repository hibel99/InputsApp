using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;


namespace InputsApp.DataAccess;

public class SqlDataAccess : ISqlDataAccess
{
    public async Task<List<T>> LoadData<T, U>(
         string StoredProcedure,
         U parameters,
         string connectionString
         )
    {
        using IDbConnection connection = new SqlConnection(connectionString);
        var rows = await connection.QueryAsync<T>(StoredProcedure, null, commandType: CommandType.StoredProcedure);
        return rows.ToList();

    }

    public async Task SaveData<T>(
        string storedProcdure,
        T parameters,
        string connectionString
        )
    {
        using IDbConnection connection = new SqlConnection(connectionString);
        await connection.ExecuteAsync(
               storedProcdure,
               parameters,
               commandType: CommandType.StoredProcedure);
    }
}
