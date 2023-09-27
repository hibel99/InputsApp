using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InputsApp.Models
{
    public class Spans
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Spans()
        {
        }

        public Spans(decimal length, decimal diameter, string category, string name, decimal cost,  string pivotID)
        {
            Length = length;
            Diameter = diameter;
            Category = category;
            Name = name;
            Cost = cost;
            PivotID = pivotID;

        }


        public Spans(int iD, decimal length, decimal diameter, string category, string name, decimal cost,   string pivotID)
        {
            ID = iD;
            Length = length;
            Diameter = diameter;
            Category = category;
            Name = name;
            Cost = cost;
            PivotID = pivotID;
        }

        public int ID { get; set; }
        public decimal Length { get; set; }
        public decimal Diameter { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public string PivotID { get; set; }
        public List<PivotTable> ParentPivots { get; set; }
    }
}
