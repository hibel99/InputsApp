using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Media3D;
using static System.Collections.Specialized.BitVector32;

namespace InputsApp.Models;

public class SpareParts
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public int ID { get; set; }
    public string PivotCategory { get; set; }
    public string Section { get; set; }
    public string PivotPart { get; set; }
    public decimal Cost { get; set; }
    public DateTime Date { get; set; }
    public decimal Height { get; set; }
    public decimal Width { get; set; }
    public decimal Length { get; set; }
    public decimal Weight { get; set; }
    public string pivotcode { get; set; }
    public int PartLevel { get; set; }
    public string SetID { get; set; }
    public string SpareID { get; set; }
    public double Quantity { get; set; }
    public string SpanID { get; set; }
    public string NameAR { get; set; }
    public string Brand { get; set; }
    public List<PivotTable> ParentPivots { get; set; }
    public List<SpareParts> ParentSpares { get; set; }
    public List<Spans> ParentSpans { get; set; }
    public string Name => NameAR;


    public SpareParts()
    {

    }

    public SpareParts(string pivotCategory, string pivotPart, decimal cost, 
        DateTime date, decimal height, decimal width, decimal length, 
        decimal weight, string PivotCode,int partLevel, string setID, 
        string spareID, double quantity, string spanID, string nameAR,string section)
    {
        PivotCategory = pivotCategory;
        PivotPart = pivotPart;
        Cost = cost;
        Date = date;
        Height = height;
        Width = width;
        Length = length;
        Weight = weight;
        pivotcode = PivotCode;
        PartLevel = partLevel;
        SetID = setID;
        SpareID = spareID;
        Quantity = quantity;
        SpanID = spanID;
        NameAR = nameAR;
        Section = section;
    }

    public SpareParts(int iD, string pivotCategory, string pivotPart, decimal cost, 
        DateTime date, decimal height, decimal width, decimal length, decimal weight,
        string PivotCode, int partLevel, string setID, string spareID, double quantity, string spanID, string nameAR, string section)
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
        pivotcode = PivotCode;
        PartLevel = partLevel;
        SetID = setID;
        SpareID = spareID;
        Quantity = quantity;
        SpanID = spanID;
        NameAR = nameAR;
        Section = section;

    }

}
