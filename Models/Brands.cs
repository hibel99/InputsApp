using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.Models
{
    public class Brands
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Brands()
        {

        }
        public Brands(string category, string brand)
        {
            Category = category;
            Brand = brand;
        }


        public Brands(string category, string brand, int iD)
        {
            Category = category;
            Brand = brand;
            ID = iD;
        }

        public int ID { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
