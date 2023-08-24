using Dapper;
using InputsApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InputsApp.DataAccess;

public class SprinklerPartsRepository : ISprinklerPartsRepository
{

    private readonly ISqlDataAccess _sqlDataAccess;

    public SprinklerPartsRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<SprinklerParts>> GetSprinklerParts()
    {
        try
        {
            var result = await _sqlDataAccess.LoadData<SprinklerParts, dynamic>("dbo.spSelectSprinklerParts",
                new { },
                AppConnection.ConnectionString);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to retrive the Sprinkler Parts from Database", ex);
        }
    }

    public async Task AddSprinklerPart(SprinklerParts sprinklerPart)
    {
        try
        {
            await _sqlDataAccess.SaveData<dynamic>("dbo.spAddNewSprinklerParts",
              new
              {
                  SprinklerCategory = sprinklerPart.SprinklerCategory,
                  SprinklerPart = sprinklerPart.SprinklerPart,
                  Cost = sprinklerPart.Cost,
                  Date = sprinklerPart.Date,
                  Height = sprinklerPart.Height,
                  Width = sprinklerPart.Width,
                  Length = sprinklerPart.Length,
                  Weight = sprinklerPart.Weight,
                  PivotID = sprinklerPart.PivotID
              },
              AppConnection.ConnectionString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to Insert the Sprinkler  part into Database using method: {nameof(AddSprinklerPart)}", ex);
        }
    }


    public async Task EditSprinklerPart(SprinklerParts sprinklerPart)
    {
        await _sqlDataAccess.SaveData<dynamic>("dbo.spEditSprinklerParts",
         new
         {
             ID = sprinklerPart.ID,
             SprinklerCategory = sprinklerPart.SprinklerCategory,
             SprinklerPart = sprinklerPart.SprinklerPart,
             Cost = sprinklerPart.Cost,
             Date = sprinklerPart.Date,
             Height = sprinklerPart.Height,
             Length = sprinklerPart.Length,
             Width = sprinklerPart.Width,
             Weight = sprinklerPart.Weight
         },
            AppConnection.ConnectionString);
    }
    public async Task DeleteSprinklerPart(int sprinklerPartId)
    {
        try
        {
            await _sqlDataAccess.SaveData<dynamic>("dbo.spDeleteSprinklerParts",
                new
                {
                    ID = sprinklerPartId,
                },
               AppConnection.ConnectionString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete the sprinkler part with this ID: {sprinklerPartId}, from Database using method: {nameof(DeleteSprinklerPart)}", ex);
        }

    }

}
