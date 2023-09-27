using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.Models
{
    public class Categories
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Categories()
        {

        }
        public Categories(string type, string name, string nameAR)
        {
            Type = type;
            Name = name;
            NameAR = nameAR;
        }


        public Categories(string type, string name, string nameAR, int iD)
        {
            Type = type;
            Name = name;
            NameAR = nameAR;
            ID = iD;
        }

        public int ID { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string NameAR { get; set; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
