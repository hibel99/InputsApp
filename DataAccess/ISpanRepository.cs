using InputsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace InputsApp.DataAccess
{
    public interface ISpanRepository
    {
        Task<List<Spans>> GetSpans();
        Task AddSpan(Spans pivot);
        Task EditSpan(Spans pivot);
        Task DeleteSpan(int pivotId);
    }
}