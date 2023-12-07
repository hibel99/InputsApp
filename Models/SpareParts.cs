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

public class SpareParts:INotifyPropertyChanged
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
    public string CostCurrency { get; set; }
    public DateTime Date { get; set; }
    public decimal Height { get; set; }
    public string? HeightUnit { get; set; }

    public decimal Width { get; set; }

    public string? WidthUnit { get; set; }
    public decimal Length { get; set; }

    public string? LengthUnit { get; set; }
    public decimal Weight { get; set; }

    public string? WeightUnit { get; set; }
    public int pivotcode { get; set; }
    public int PartLevel { get; set; }
    public int SetID { get; set; }
    public int SpareID { get; set; }
    public double Quantity { get; set; }
    public int SpanID { get; set; }
    public string NameAR { get; set; }
    public string Brand { get; set; }
    public List<SpareRelationship> ParentPivots { get; set; }
    public List<SpareRelationship> ParentSpares { get; set; }
    public List<SpareRelationship> ParentSpans { get; set; }
    public List<SpareRelationship> ParentSets { get; set; }
    public string ParentType { get; set; }
    public bool HasChild { get; set; }
    public string Name => NameAR;

    public int? PivotPartID { get; set; }




    public string? CostForDG => $"{CostCurrency}{Cost}";

    public string? PartCode => $"Part{PivotPart?.Substring(0, Math.Min(PivotPart.Length, 3))}{PivotCategory?.Substring(0, Math.Min(PivotCategory.Length, 1))}{ID}";

    public string? LengthForDG => $"{Length} {LengthUnit}";
    public string? HeightForDG => $"{Height} {HeightUnit}";

    public string? WidthForDG => $"{Width} {WidthUnit}";

    public string? WeighthForDG => $"{Weight} {WeightUnit}";






    public SpareParts()
    {

    }


    public SpareParts(string pivotCategory, string pivotPart, decimal cost, string costCurrency,
        DateTime date, decimal height, string? heightUnit, decimal width, string? widthUnit, decimal length, string? lengthUnit,
        decimal weight, string? weightUnit, int PivotCode, int partLevel, int setID,
        int spareID, double quantity, int spanID, string nameAR, string section, string brand, bool haschild = false)
    {
        PivotCategory = pivotCategory;
        PivotPart = pivotPart;
        Cost = cost;
        CostCurrency = costCurrency;
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
        HasChild = haschild;
        HeightUnit = heightUnit;
        WidthUnit = widthUnit;
        LengthUnit = lengthUnit;
        WeightUnit = weightUnit;
        
    }
    
    public SpareParts(string pivotCategory, string pivotPart, decimal cost, string costCurrency,
        DateTime date, decimal height, string? heightUnit, decimal width, string? widthUnit, decimal length, string? lengthUnit,
        decimal weight, string? weightUnit,int PivotCode,int partLevel, int setID,
        int spareID, int spanID, string nameAR,string section,string brand,bool haschild = false)
    {
        PivotCategory = pivotCategory;
        PivotPart = pivotPart;
        Cost = cost;
        CostCurrency = costCurrency;
        Date = date;
        Height = height;
        Width = width;
        Length = length;
        Weight = weight;
        pivotcode = PivotCode;
        PartLevel = partLevel;
        SetID = setID;
        SpareID = spareID;
        SpanID = spanID;
        NameAR = nameAR;
        Section = section;
        Brand = brand;
        HasChild = haschild;
        HeightUnit = heightUnit;
        WidthUnit = widthUnit;
        LengthUnit = lengthUnit;
        WeightUnit = weightUnit;

    }

    public SpareParts(int iD, string pivotCategory, string pivotPart, decimal cost, string costCurrency,
        DateTime date, decimal height, string? heightUnit, decimal width, string? widthUnit, decimal length, string? lengthUnit,
        decimal weight, string? weightUnit,
        int PivotCode, int partLevel, int setID, int spareID, double quantity, int spanID, string nameAR, string section, string brand, bool haschild = false)
    {
        ID = iD;
        PivotCategory = pivotCategory;
        PivotPart = pivotPart;
        Cost = cost;
        CostCurrency = costCurrency;
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
        HasChild = haschild;
        HeightUnit = heightUnit;
        WidthUnit = widthUnit;
        LengthUnit = lengthUnit;
        WeightUnit = weightUnit;

    }

    private ObservableCollection<SpareRelationship> _pivotParentOBS;
    public ObservableCollection<SpareRelationship> PivotParentOBS
    {
        get { return _pivotParentOBS; }
        set
        {
            if (_pivotParentOBS != value)
            {
                _pivotParentOBS = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<SpareRelationship> _spanParentOBS;
    public ObservableCollection<SpareRelationship> SpanParentOBS
    {
        get { return _spanParentOBS; }
        set
        {
            if (_spanParentOBS != value)
            {
                _spanParentOBS = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<SpareRelationship> _spareParentOBS;
    public ObservableCollection<SpareRelationship> SpareParentOBS
    {
        get { return _spareParentOBS; }
        set
        {
            if (_spareParentOBS != value)
            {
                _spareParentOBS = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<SpareRelationship> _setParentOBS;
    public ObservableCollection<SpareRelationship> SetParentOBS
    {
        get { return _setParentOBS; }
        set
        {
            if (_setParentOBS != value)
            {
                _setParentOBS = value;
                OnPropertyChanged();
            }
        }
    }
}

