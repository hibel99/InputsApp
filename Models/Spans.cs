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
        public Spans()
        {
        }

        public Spans(decimal length, decimal diameter, string category, string name, decimal cost, string pipeType, string overhangType)
        {
            Length = length;
            Diameter = diameter;
            Category = category;
            Name = name;
            Cost = cost;
            PipeType = pipeType;
            OverhangType = overhangType;
        }


        public Spans(int iD, decimal length, decimal diameter, string category, string name, decimal cost, string pipeType, string overhangType)
        {
            ID = iD;
            Length = length;
            Diameter = diameter;
            Category = category;
            Name = name;
            Cost = cost;
            PipeType = pipeType;
            OverhangType = overhangType;
        }

        public int ID { get; set; }
        public decimal Length { get; set; }
        public decimal Diameter { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public string PipeType { get; set; }
        public string OverhangType { get; set; }
    }
}
