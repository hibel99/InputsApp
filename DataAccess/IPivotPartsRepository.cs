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
        Task<List<PivotParts>> GetPivotParts();
        Task AddPivotPart(PivotParts pivotPart);
        Task EditPivotPart(PivotParts pivotPart);
        Task DeletePivotPart(int pivotPartId);
    }
}