using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.Models;

public class Span
{

    public int ID { get; set; }
 
    public Double Length { get; set; }

    public Double Diameter { get; set; }
    public string Category { get; set; }


    public Span(double length, double diameter, string category)
    {
        Length = length;
        Diameter = diameter;
        Category = category;
    }

    public Span(int iD, double length, double diameter,string category)
    {
        ID = iD;
        Length = length;
        Diameter = diameter;
        Category = category;
    }



}
