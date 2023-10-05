using InputsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.DataAccess
{
    public interface IPivotPartsRepository
    {
        Task<List<SpareParts>> GetPivotParts();
        Task<int> AddPivotPart(SpareParts pivotPart);
        Task EditPivotPart(SpareParts pivotPart);
        Task EditPivotPart(List<SpareParts> pivotPart);
        Task DeletePivotPart(int pivotPartId);

        Task<List<SpareRelationship>> GetPivotPartsRelations();

        Task<List<SpareParts>> GetPivotPartsRelationsJoined();

        Task<SpareRelationship> AddPivotPartRelation(SpareRelationship pivotPart);
        Task<List<SpareRelationship>> AddPivotPartRelation(List<SpareRelationship> pivotPart);
        Task EditPivotPartRelation(SpareParts pivotPart);
        Task DeletePivotPartRelation(int pivotPartId);
    }
}