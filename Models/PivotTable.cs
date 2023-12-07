using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.Models
{
    public class PivotTable : INotifyPropertyChanged
    {
        public ObservableCollection<string> PivotsOBS { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public PivotTable()
        {

        }

        public PivotTable(string pivotName, string pivotCategory, decimal pivotLength)
        {
            pivotname = pivotName;
            pivotcategory = pivotCategory;
            pivotlength = pivotLength;
        }


        public PivotTable(decimal pivotLength, string pivotName, string pivotCategory, int iD)
        {
            pivotlength = pivotLength;
            pivotname = pivotName;
            pivotcategory = pivotCategory;
            ID = iD;
        }

        
        public string Name => pivotname;

        private string _pivotname;

        public string pivotname
        {
            get { return _pivotname; }
            set
            {
                if (_pivotname != value)
                {
                    _pivotname = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _pivotcategory;
        public string pivotcategory
        {
            get { return _pivotcategory; }
            set
            {
                if (_pivotcategory != value)
                {
                    _pivotcategory = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _ID;
        public int ID
        {
            get { return _ID; }
            set
            {
                if (_ID != value)
                {
                    _ID = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _pivotlength;

        public decimal pivotlength
    {
            get { return _pivotlength; }
            set
            {
                if (_pivotlength != value)
                {
                    _pivotlength = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? PivotCode => $"PIV{pivotname?.Substring(0, Math.Min(pivotname.Length, 3))}{ID}";


    }
}
