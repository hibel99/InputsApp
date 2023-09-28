
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

public class SpanRepository : ISpanRepository
{

    private readonly ISqlDataAccess _sqlDataAccess;

    public SpanRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<Spans>> GetSpans()
    {
        try
        {
            var result = await _sqlDataAccess.LoadData<Spans, dynamic>("dbo.spSelectSpan",
                new { },
                AppConnection.ConnectionString);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to retrive the Spans from Database", ex);
        }
    }

    public async Task<int> AddSpan(Spans span)
    {
        try
        {
            var result = await _sqlDataAccess.LoadData<int, dynamic>("dbo.spAddSpan",
               new
               {
                   Length = span.Length,
                   Diameter = span.Diameter,
                   Category = span.Category,
                   Name = span.Name,
                   Cost = span.Cost,
                   PivotID = span.PivotID,
               },
               AppConnection.ConnectionString);
            return result.FirstOrDefault();

        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to Insert the pivot parts into Database using method: {nameof(AddSpan)}", ex);
        }

    }



    public async Task EditSpan(Spans span)
    {
        await _sqlDataAccess.SaveData<dynamic>("dbo.spEditSpan",
     new
     {
         ID = span.ID,
         Length = span.Length,
         Diameter = span.Diameter,
         Category = span.Category,
         Name = span.Name,
         Cost = span.Cost,
         PivotID = span.PivotID,
     },
    AppConnection.ConnectionString);
    }




    public async Task DeleteSpan(int spanId)
    {
        try
        {
            await _sqlDataAccess.SaveData<dynamic>("dbo.spDeleteSpan",
                new
                {
                    ID = spanId,
                },
               AppConnection.ConnectionString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete the pivot part with this ID: {spanId}, from Database using method: {nameof(DeleteSpan)}", ex);
        }

    }

}