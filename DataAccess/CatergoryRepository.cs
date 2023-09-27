using InputsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.DataAccess
{
    public class CatergoryRepository : ICatergoryRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public CatergoryRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task AddCategories(Categories categories)
        {
            await _sqlDataAccess.SaveData<dynamic>("dbo.spAddCategory",
                       new
                       {
                           Type = categories.Type,
                           Name = categories.Name,
                           NameAR = categories.NameAR,
                       },
                          AppConnection.ConnectionString);
        }

        public async Task DeleteCategories(Categories categories)
        {
            await _sqlDataAccess.SaveData<dynamic>("dbo.spDeleteCategory",
            new
            {
                ID = categories.ID,

            },
             AppConnection.ConnectionString);
        }

        public async Task EditCategories(Categories categories)
        {
            await _sqlDataAccess.SaveData<dynamic>("dbo.spEditCategory",
            new
            {
                ID = categories.ID,
                Type = categories.Type,
                Name = categories.Name,
                NameAR = categories.NameAR,

            },
               AppConnection.ConnectionString);
        }

        public async Task<List<Categories>> GetCategories()
        {
            try
            {
                List<Categories> result = await _sqlDataAccess.LoadData<Categories, dynamic>("dbo.spSelectCategory",
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
