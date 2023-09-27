using InputsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.DataAccess
{
    public interface ICatergoryRepository
    {
        Task<List<Categories>> GetCategories();
        Task AddCategories(Categories categories);
        Task EditCategories(Categories categories);
        Task DeleteCategories(Categories categories);
    }
}
