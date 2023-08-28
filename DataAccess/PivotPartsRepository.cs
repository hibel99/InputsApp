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
using System.Windows.Media.Media3D;

namespace InputsApp.DataAccess;

public class PivotPartsRepository : IPivotPartsRepository
{

    private readonly ISqlDataAccess _sqlDataAccess;

    public PivotPartsRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<PivotParts>> GetPivotParts()
    {
        try
        {
            var result = await _sqlDataAccess.LoadData<PivotParts, dynamic>("dbo.spSelectPivotParts",
                new { },
                AppConnection.ConnectionString);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to retrive the pivot parts from Database", ex);
        }
    }

    public async Task AddPivotPart(PivotParts pivotPart)
    {
        try
        {
            await _sqlDataAccess.SaveData<dynamic>("dbo.spAddNewPivotPart",
               new
               {
                   PivotCategory = pivotPart.PivotCategory,
                   PivotPart = pivotPart.PivotPart,
                   Cost = pivotPart.Cost,
                   Date = pivotPart.Date,
                   Height = pivotPart.Height,
                   Length = pivotPart.Length,
                   Width = pivotPart.Width,
                   Weight = pivotPart.Weight,
                   PivotCode = pivotPart.pivotcode
               },
               AppConnection.ConnectionString);

        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to Insert the pivot parts into Database using method: {nameof(AddPivotPart)}", ex);
        }

    }



    public async Task EditPivotPart(PivotParts pivotPart)
    {
        await _sqlDataAccess.SaveData<dynamic>("dbo.spEditPivotPart",
 new
 {
     ID = pivotPart.ID,
     PivotCategory = pivotPart.PivotCategory,
     PivotPart = pivotPart.PivotPart,
     Cost = pivotPart.Cost,
     Date = pivotPart.Date,
     Height = pivotPart.Height,
     Length = pivotPart.Length,
     Width = pivotPart.Width,
     Weight = pivotPart.Weight,
     PivotCode = pivotPart.pivotcode
 },
    AppConnection.ConnectionString);
    }




    public async Task DeletePivotPart(int pivotPartId)
    {
        try
        {
            await _sqlDataAccess.SaveData<dynamic>("dbo.spDeletePivotParts",
                new
                {
                    ID = pivotPartId,
                },
               AppConnection.ConnectionString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete the pivot part with this ID: {pivotPartId}, from Database using method: {nameof(DeletePivotPart)}", ex);
        }

    }

}
