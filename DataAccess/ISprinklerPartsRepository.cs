using InputsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.DataAccess
{
    public interface ISprinklerPartsRepository
    {
        Task<List<SprinklerParts>> GetSprinklerParts();
        Task<int> AddSprinklerPart(SprinklerParts sprinklerPart);
        Task EditSprinklerPart(SprinklerParts sprinklerPart);
        Task DeleteSprinklerPart(int sprinklerPartId);
    }
}