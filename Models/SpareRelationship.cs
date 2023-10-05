using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InputsApp.Models
{
    public class SpareRelationship:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public int ID { get; set; } 
        public int pivotcode { get; set; }
        public int PartLevel { get; set; }
        public int SetID { get; set; }
        public int SpareID { get; set; }
        public int PivotPartID { get; set; }
        public double Quantity { get; set; }
        public int SpanID { get; set; }
        public string ParentType { get; set; }

        public string PivotCategory { get; set; }
        public string PivotPart { get; set; }

        public SpareRelationship() { }
        public SpareRelationship(string pivotCategory, string pivotPart,
        int PivotCode, int partLevel, int setID,
        int spareID, double quantity, int spanID,string parentType)
        {
            PivotCategory = pivotCategory;
            PivotPart = pivotPart;
            pivotcode = PivotCode;
            PartLevel = partLevel;
            SetID = setID;
            SpareID = spareID;
            Quantity = quantity;
            SpanID = spanID;
            ParentType = parentType;
            PivotCategory = pivotCategory;
            PivotPart = pivotPart;

        }

        public SpareRelationship(int id,string pivotCategory, string pivotPart,
        int PivotCode, int partLevel, int setID,
        int spareID, double quantity, int spanID, string parentType)
        {
            ID = id;
            PivotCategory = pivotCategory;
            PivotPart = pivotPart;
            pivotcode = PivotCode;
            PartLevel = partLevel;
            SetID = setID;
            SpareID = spareID;
            Quantity = quantity;
            SpanID = spanID;
            ParentType = parentType;
            PivotCategory = pivotCategory;
            PivotPart = pivotPart;

        }


    }
}
