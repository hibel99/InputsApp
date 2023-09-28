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
        Task DeletePivotPart(int pivotPartId);
    }
}