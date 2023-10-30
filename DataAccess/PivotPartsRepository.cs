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

    public async Task<List<SpareParts>> GetPivotParts()
    {
        try
        {
            var result = await _sqlDataAccess.LoadData<SpareParts, dynamic>("dbo.spSelectPivotParts",
                new { },
                AppConnection.ConnectionString);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to retrive the pivot parts from Database", ex);
        }
    }

    public async Task<int> AddPivotPart(SpareParts pivotPart)
    {
        try
        {
            var result = await _sqlDataAccess.LoadData<int,dynamic>("dbo.spAddNewPivotPart",
               new
               {
                   PivotCategory = pivotPart.PivotCategory,
                   PivotPart = pivotPart.PivotPart,
                   Cost = pivotPart.Cost,
                   Date = pivotPart.Date,
                   Height = pivotPart.Height,
                   HeightUnit = pivotPart.HeightUnit,
                   Width = pivotPart.Width,
                   WidthUnit = pivotPart.WidthUnit,
                   Length = pivotPart.Length,
                   LengthUnit = pivotPart.LengthUnit,
                   Weight = pivotPart.Weight,
                   WeightUnit = pivotPart.WeightUnit,
                   NameAR = pivotPart.NameAR,
                   section = pivotPart.Section,
                   Brand = pivotPart.Brand,
                   HasChild = pivotPart.HasChild,   
        },
               AppConnection.ConnectionString);
            return result.FirstOrDefault();

        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to Insert the pivot parts into Database using method: {nameof(AddPivotPart)}", ex);
        }

    }

    public async Task EditPivotPart(SpareParts pivotPart)
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
             HeightUnit = pivotPart.HeightUnit,
             Width = pivotPart.Width,
             WidthUnit = pivotPart.WidthUnit,
             Length = pivotPart.Length,
             LengthUnit = pivotPart.LengthUnit,
             Weight = pivotPart.Weight,
             WeightUnit = pivotPart.WeightUnit,
             NameAR = pivotPart.NameAR,
             Section = pivotPart.Section,
             Brand = pivotPart.Brand,
             HasChild = pivotPart.HasChild,
         
         },
    AppConnection.ConnectionString);
    }

    public async Task EditPivotPart(List< SpareParts> pivotPart)
    {
        foreach (var item in pivotPart)
        {
            await _sqlDataAccess.SaveData<dynamic>("dbo.spEditPivotPart",
   new
            {
                ID = item.ID,
                PivotCategory = item.PivotCategory,
                PivotPart = item.PivotPart,
                Cost = item.Cost,
                Date = item.Date,
                Height = item.Height,
                Length = item.Length,
                Width = item.Width,
                Weight = item.Weight,
                NameAR = item.NameAR,
                Brand = item.Brand,
                Section = item.Section,
                HasChild = item.HasChild,
                HeightUnit = item.HeightUnit,
                WidthUnit = item.WidthUnit,
                LengthUnit = item.LengthUnit,
                WeightUnit = item.WeightUnit,
   },
      AppConnection.ConnectionString); 
        }
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

    public async Task<List<SpareRelationship>> GetPivotPartsRelations()
    {
        try
        {
            var result = await _sqlDataAccess.LoadData<SpareRelationship, dynamic>("dbo.spSelectRelations",
                new { },
                AppConnection.ConnectionString);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to retrive the pivot parts from Database", ex);
        }
    }

    public async Task<int> AddPivotPartRelation(SpareRelationship pivotPart)
    {
        try
        {
            var result = await _sqlDataAccess.LoadData<int, dynamic>("dbo.spAddRelation",
               new
               {
                   PivotCode = pivotPart.pivotcode,
                   PartLevel = pivotPart.PartLevel,
                   SetID = pivotPart.SetID,
                   SpareID = pivotPart.SpareID,
                   Quantity = pivotPart.Quantity,
                   SpanID = pivotPart.SpanID,
                   PivotPartID = pivotPart.PivotPartID,
                   PivotCategory = pivotPart.PivotCategory,
                   PivotPart = pivotPart.PivotPart,
                   ParentType = pivotPart.ParentType,

               },
               AppConnection.ConnectionString);
   
            return result.FirstOrDefault();

        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to Insert the pivot parts into Database using method: {nameof(AddPivotPart)}", ex);
        }

    }

    public async Task<List<SpareRelationship>> AddPivotPartRelation(List<SpareRelationship> pivotParts)
    {
        try
        {
            foreach (var item in pivotParts)
            {
                var result = await _sqlDataAccess.LoadData<int, dynamic>("dbo.spAddRelation",
                     new
                     {

                         PivotCode = item.pivotcode,
                         PartLevel = item.PartLevel,
                         SetID = item.SetID,
                         SpareID = item.SpareID,
                         Quantity = item.Quantity,
                         SpanID = item.SpanID,
                         PivotPartID = item.PivotPartID,
                         PivotCategory = item.PivotCategory,
                         PivotPart = item.PivotPart,
                         ParentType = item.ParentType,

                     },
                     AppConnection.ConnectionString);
                item.ID = result.FirstOrDefault();
            }
            return pivotParts;

        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to Insert the pivot parts into Database using method: {nameof(AddPivotPart)}", ex);
        }

    }


    public async Task EditPivotPartRelation(SpareRelationship pivotPart)
    {
        await _sqlDataAccess.SaveData<dynamic>("dbo.spEditRelation",
         new
         {
              ID = pivotPart.ID,
             PivotCode = pivotPart.pivotcode,
             PartLevel = pivotPart.PartLevel,
             SetID = pivotPart.SetID,
             SpareID = pivotPart.SpareID,
             Quantity = pivotPart.Quantity,
             SpanID = pivotPart.SpanID,
             PivotPartID = pivotPart.PivotPartID,
         },
    AppConnection.ConnectionString);
    }

    public async Task DeletePivotPartRelation(int pivotPartId)
    {
        try
        {
            await _sqlDataAccess.SaveData<dynamic>("dbo.spDeleteRelation",
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

    public async Task<List<SpareParts>> GetPivotPartsRelationsJoined()
    {
        try
        {
            var result = await _sqlDataAccess.LoadData<SpareParts, dynamic>("dbo.spSelectJoinedSpares",
                new { },
                AppConnection.ConnectionString);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to retrive the pivot parts from Database", ex);
        }
    }
}
