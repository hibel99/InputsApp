using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.Models
{
    public class Spans
    {
        public Spans(decimal length, decimal diameter, string category)
        {
            Length = length;
            Diameter = diameter;
            Category = category;
        }


        public Spans(int iD, decimal length, decimal diameter, string category)
        {
            ID = iD;
            Length = length;
            Diameter = diameter;
            Category = category;
        }

        public int ID { get; set; }
        public decimal Length { get; set; }
        public decimal Diameter { get; set; }
        public string Category { get; set; }
    }
}
