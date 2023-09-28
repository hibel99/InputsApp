using InputsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.DataAccess
{
    public interface IPivotRepository
    {
        Task<List<PivotTable>> GetPivots();
        Task<int> AddPivot(PivotTable pivot);
        Task EditPivot(PivotTable pivot);
        Task DeletePivot(int pivotId);
    }
}