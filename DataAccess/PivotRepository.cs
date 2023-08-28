﻿using Dapper;
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

public class PivotRepository : IPivotRepository
{

    private readonly ISqlDataAccess _sqlDataAccess;

    public PivotRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<Pivot>> GetPivots()
    {
        try
        {
            var result = await _sqlDataAccess.LoadData<Pivot, dynamic>("dbo.spSelectPivot",
                new { },
                AppConnection.ConnectionString);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to retrive the pivot parts from Database", ex);
        }
    }

    public async Task AddPivot(Pivot pivot)
    {
        try
        {
            await _sqlDataAccess.SaveData<dynamic>("dbo.spAddNewPivot",
               new
               {
                   pivotname = pivot.pivotname,
                   pivotcategory = pivot.pivotcategory
               },
               AppConnection.ConnectionString);

        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to Insert the pivot parts into Database using method: {nameof(AddPivot)}", ex);
        }

    }



    public async Task EditPivot(Pivot pivot)
    {
        await _sqlDataAccess.SaveData<dynamic>("dbo.spEditPivot",
     new
     {
         ID = pivot.ID,
         pivotname = pivot.pivotname,
         pivotcategory = pivot.pivotcategory
     },
    AppConnection.ConnectionString);
    }




    public async Task DeletePivot(int pivotId)
    {
        try
        {
            await _sqlDataAccess.SaveData<dynamic>("dbo.spDeletePivot",
                new
                {
                    ID = pivotId,
                },
               AppConnection.ConnectionString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete the pivot part with this ID: {pivotId}, from Database using method: {nameof(DeletePivot)}", ex);
        }

    }

}
