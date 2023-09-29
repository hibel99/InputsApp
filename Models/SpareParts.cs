using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public string ParentType { get; set; }
    public string Name => NameAR;


    public SpareParts()
    {

    }

  
    public SpareParts(string pivotCategory, string pivotPart, decimal cost, 
        DateTime date, decimal height, decimal width, decimal length, 
        decimal weight, string PivotCode,int partLevel, string setID, 
        string spareID, double quantity, string spanID, string nameAR,string section,string brand)
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
        Brand = brand;
    }

    public SpareParts(int iD, string pivotCategory, string pivotPart, decimal cost, 
        DateTime date, decimal height, decimal width, decimal length, decimal weight,
        string PivotCode, int partLevel, string setID, string spareID, double quantity, string spanID, string nameAR, string section, string brand)
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
        Brand = brand;

    }

}


public static class SparePartsExtensions
{

    public static ObservableCollection<SpareParts> JoinSpanIdPivotCodeSpareId(this IEnumerable<SpareParts> spareParts)
    {
        var groupedSpareParts = spareParts.GroupBy(s => new
        {
            s.PivotCategory,
            s.Date,
            s.Section,
            s.PivotPart,
            s.NameAR,
            s.Cost,
            s.Height,
            s.Width,
            s.Length,
            s.Weight
        });

        var joinedSpareParts = groupedSpareParts.Select(g => new SpareParts
        {

            PivotCategory = g.Key.PivotCategory,
            Date = g.Key.Date,
            Section = g.Key.Section,
            PivotPart = g.Key.PivotPart,
            Cost = g.Key.Cost,
            Height = g.Key.Height,
            Width = g.Key.Width,
            Length = g.Key.Length,
            Weight = g.Key.Weight,
            NameAR = g.Key.NameAR,
            SpanID = string.Join(",", g.Where(x => x.SpanID != "").Select(s => s.SpanID)),
            pivotcode = string.Join(",", g.Where(x => x.pivotcode != "").Select(s => s.pivotcode)),
            SpareID = string.Join(",", g.Where(x => x.SpareID != "").Select(s => s.SpareID))
        });
        ObservableCollection<SpareParts> values = new ObservableCollection<SpareParts>();
        foreach (var item in joinedSpareParts)
        {
            values.Add(item);
        }
        return values;

    }

}
