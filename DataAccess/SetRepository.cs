
using Dapper;
using InputsApp.DataAccess;
using InputsApp.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Media3D;

namespace InputsApp.DataAccess;

public class SetRepository : ISetRepository
{

    private readonly ISqlDataAccess _sqlDataAccess;

    public SetRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<Set>> GetSets()
    {
        try
        {
            var result = await _sqlDataAccess.LoadData<Set, dynamic>("dbo.spSelectSets",
                new { },
                AppConnection.ConnectionString);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to retrive the Set from Database", ex);
        }
    }

    public async Task<int> AddSet(Set Set)
    {
        try
        {
            var result = await _sqlDataAccess.LoadData<int, dynamic>("dbo.spAddSet",
               new
               {
                   Name = Set.Name,
                   NameAR = Set.NameAR,

               },
               AppConnection.ConnectionString);
            return result.FirstOrDefault();

        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to Insert the pivot parts into Database using method: {nameof(AddSet)}", ex);
        }

    }



    public async Task EditSet(Set Set)
    {
        await _sqlDataAccess.SaveData<dynamic>("dbo.spEditSet",
     new
     {
         ID = Set.ID,
         Name = Set.Name,
         NameAR = Set.NameAR,
     },
    AppConnection.ConnectionString);
    }




    public async Task DeleteSet(int SetId)
    {
        try
        {
            await _sqlDataAccess.SaveData<dynamic>("dbo.spDeleteSet",
                new
                {
                    ID = SetId,
                },
               AppConnection.ConnectionString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete the pivot part with this ID: {SetId}, from Database using method: {nameof(DeleteSet)}", ex);
        }

    }

}