using InputsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace InputsApp.DataAccess
{
    public interface ISetRepository
    {
        Task<List<Set>> GetSets();
        Task<int> AddSet(Set set);
        Task EditSet(Set set);
        Task DeleteSet(int setId);
    }
}