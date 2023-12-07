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

        public Spans(decimal length, decimal diameter, string category, string name, decimal cost, string costCurrency, int outlets, decimal heightFromGround, string heightFromGroundUnit, string pivotID, string spanFlow)
        {
            Length = length;
            Diameter = diameter;
            Category = category;
            Name = name;
            Cost = cost;
            CostCurrency = costCurrency;
            Outlets = outlets;
            HeightFromGround = heightFromGround;
            HeightFromGroundUnit = heightFromGroundUnit;
            PivotID = pivotID;
            SpanFlow = spanFlow;
        }


        public Spans(int iD, decimal length, decimal diameter, string category, string name, decimal cost, string costCurrency, int outlets, decimal heightFromGround, string heightFromGroundUnit, string pivotID, string spanFlow)
        {
            ID = iD;
            Length = length;
            Diameter = diameter;
            Category = category;
            Name = name;
            Cost = cost;
            CostCurrency = costCurrency;
            Outlets = outlets;
            HeightFromGround = heightFromGround;
            HeightFromGroundUnit = heightFromGroundUnit;
            PivotID = pivotID;
            SpanFlow = spanFlow;
        }

        public int ID { get; set; }
        public decimal Length { get; set; }
        public decimal Diameter { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }

        public int Outlets { get; set; }
        public decimal HeightFromGround { get; set; }

        public string HeightFromGroundUnit { get; set; }

        public string CostCurrency { get; set; }

        public string? SpanCode => $"SP{Name?.Substring(0, Math.Min(Name.Length, 3))}{Category?.Substring(0, Math.Min(Category.Length, 1))}{ID}";

        public string? HeightFromGroundForDG => $"{HeightFromGround} {HeightFromGroundUnit}";


        public string? LengthForDG => $"{Length} m";

        public string? DiameterForDG => $"{Diameter} inch";


        public string? CostForDG => $"{CostCurrency}{Cost}";

        public string PivotID { get; set; }
        public string SpanFlow { get; set; }
        public List<PivotTable> ParentPivots { get; set; }


    }
}
