using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InputsApp.Models;

public class Set
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public int ID { get; set; }
    public string Name { get; set; }
    public string NameAR { get; set; }
    public string Category { get; set; }
    public string? NameDG => $"{Category} {Name}";


    public Set()
    {
    }

    public Set(string name,  string nameAR, string category)
    {
        Name = name;
        NameAR = nameAR;
        Category = category;
    }

    public Set(int iD, string name,string nameAR, string category)
    {
        ID = iD;
        Name = name;
        NameAR = nameAR;
        Category = category;
    }
}
