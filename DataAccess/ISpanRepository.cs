using InputsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.DataAccess
{
    public interface ISpanRepository
    {
        Task<List<Span>> GetSpans();
        Task AddSpan(Span pivot);
        Task EditSpan(Span pivot);
        Task DeleteSpan(int pivotId);
    }
}
