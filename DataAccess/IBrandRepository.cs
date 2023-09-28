using InputsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.DataAccess
{
    public interface IBrandRepository
    {
        Task<List<Brands>> GetBrands();
        Task<int> AddBrands(Brands brands);
        Task EditBrands(Brands brands);
        Task DeleteBrands(Brands brands);
    }
}
