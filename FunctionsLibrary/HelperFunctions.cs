using InputsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.FunctionsLibrary
{
    public class HelperFunctions
    {
        public static bool CompareSpareParts(SpareParts x, SpareParts y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }
            if (x.ID == 30)
            {

            }
            return x.PivotCategory == y.PivotCategory &&
                   x.Section == y.Section &&
                   x.PivotPart == y.PivotPart &&
                   x.Cost == y.Cost &&
                   x.Height == y.Height &&
                   x.Width == y.Width &&
                   x.Length == y.Length &&
                   x.PivotPart == y.PivotPart &&
                   x.NameAR == y.NameAR &&
                   x.Brand == y.Brand &&
                   x.PartLevel == y.PartLevel &&
                   x.Weight == y.Weight;
        }

    }
}
