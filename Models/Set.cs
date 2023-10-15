using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.Models;

public class Set
{

    public int ID { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public string NameAR { get; set; }


    public Set()
    {
    }

    public Set(string name, string category, string nameAR)
    {
        Name = name;
        Category = category;
        NameAR = nameAR;
    }

    public Set(int iD, string name, string category,string nameAR)
    {
        ID = iD;
        Name = name;
        Category = category;
        NameAR = nameAR;
    }
}
