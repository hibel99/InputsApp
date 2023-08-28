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
        Task<List<Pivot>> GetPivots();
        Task AddPivot(Pivot pivot);
        Task EditPivot(Pivot pivot);
        Task DeletePivot(int pivotId);
    }
}