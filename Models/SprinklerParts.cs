using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.Models
{
    public class SprinklerParts
    {
        public SprinklerParts(string sprinklerCategory, string sprinklerPart, decimal cost, DateTime date, decimal height, decimal width, decimal length, decimal weight)
        {
            SprinklerCategory = sprinklerCategory;
            SprinklerPart = sprinklerPart;
            Cost = cost;
            Date = date;
            Height = height;
            Width = width;
            Length = length;
            Weight = weight;
        }


        public SprinklerParts(int iD, string sprinklerCategory, string sprinklerPart, decimal cost, DateTime date, decimal height, decimal length, decimal width, decimal weight)
        {
            ID = iD;
            SprinklerCategory = sprinklerCategory;
            SprinklerPart = sprinklerPart;
            Cost = cost;
            Date = date;
            Height = height;
            Width = width;
            Length = length;
            Weight = weight;
        }

        public int ID { get; set; }
        public string SprinklerCategory { get; set; }
        public string SprinklerPart { get; set; }
        public decimal Cost { get; set; }
        public DateTime Date { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal Weight { get; set; }
    }
}