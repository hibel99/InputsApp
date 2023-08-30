using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InputsApp.Models
{
    public class Spans
    {
        public Spans(decimal length, decimal diameter, string category, string name)
        {
            Length = length;
            Diameter = diameter;
            Category = category;
            Name = name;
        }


        public Spans(int iD, decimal length, decimal diameter, string category, string name)
        {
            ID = iD;
            Length = length;
            Diameter = diameter;
            Category = category;
            Name = name;
        }

        public int ID { get; set; }
        public decimal Length { get; set; }
        public decimal Diameter { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
    }
}
