using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.Models
{
    public class SprinklerParts
    {
        public int ID { get; set; }
        public int InvoiceID { get; set; }
        public string SprinklerCategory { get; set; }
        public string SprinklerPart { get; set; }
        public int Quantity { get; set; }
        public decimal Cost { get; set; }
        public decimal MarkupPercentage { get; set; }
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
        public decimal SprinklerPackageListPrice { get; set; }
    }
}
