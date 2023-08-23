using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.DataAccess;

public interface ISqlDataAccess
{
    Task<List<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionString);

    Task SaveData<T>(string storedProcdure, T parameters, string connectionString);
}