using InputsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.DataAccess
{
    class BrandRepository:IBrandRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public BrandRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<int> AddBrands(Brands brands)
        {
            var result = await _sqlDataAccess.LoadData<int, dynamic>("dbo.spAddBrand",
                       new
                       {
                           brands.Brand,

                           brands.Category
                       },
                          AppConnection.ConnectionString);
            return result.FirstOrDefault();

        }

        public async Task DeleteBrands(Brands brands)
        {
            await _sqlDataAccess.SaveData<dynamic>("dbo.spDeleteBrand",
            new
            {
                ID = brands.ID,

            },
             AppConnection.ConnectionString);
        }

        public async Task EditBrands(Brands brands)
        {
            await _sqlDataAccess.SaveData<dynamic>("dbo.spEditBrand",
            new
            {
                ID = brands.ID,
                Brand = brands.Brand,

                Category = brands.Category,

            },
               AppConnection.ConnectionString);
        }

        public async Task<List<Brands>> GetBrands()
        {
            try
            {
                List<Brands> result = await _sqlDataAccess.LoadData<Brands, dynamic>("dbo.spSelectBrand",
                    new { },
                    AppConnection.ConnectionString);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrive the Categories from Database", ex);
            }
        }
    }
}
