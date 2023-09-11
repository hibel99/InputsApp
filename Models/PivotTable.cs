using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.Models
{
    public class PivotTable
    {
        public PivotTable(string pivotName, string pivotCategory, decimal pivotLength)
        {
            pivotname = pivotName;
            pivotcategory = pivotCategory;
            pivotlength = pivotLength;
        }


        public PivotTable(decimal pivotLength, string pivotName, string pivotCategory, int iD)
        {
            pivotlength = pivotLength;
            pivotname = pivotName;
            pivotcategory = pivotCategory;
            ID = iD;
        }

        public string pivotname { get; set; }
        public string pivotcategory { get; set; }
        public int ID { get; set; }
        public decimal pivotlength { get; set; }
    }
}
