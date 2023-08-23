using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.Models;

public class PivotParts
{

    public int ID { get; set; }
    public string PivotCategory { get; set; }
    public string PivotPart { get; set; }
    public decimal Cost { get; set; }
    public DateTime Date { get; set; }
    public decimal Height { get; set; }
    public decimal Width { get; set; }
    public decimal Length { get; set; }
    public decimal Weight { get; set; }


    public PivotParts(string pivotCategory, string pivotPart, decimal cost, DateTime date, decimal height, decimal width, decimal length, decimal weight)
    {
        PivotCategory = pivotCategory;
        PivotPart = pivotPart;
        Cost = cost;
        Date = date;
        Height = height;
        Width = width;
        Length = length;
        Weight = weight;
    }

    public PivotParts(int iD, string pivotCategory, string pivotPart, decimal cost, DateTime date, decimal height, decimal width, decimal length, decimal weight)
    {
        ID = iD;
        PivotCategory = pivotCategory;
        PivotPart = pivotPart;
        Cost = cost;
        Date = date;
        Height = height;
        Width = width;
        Length = length;
        Weight = weight;
    }

}
