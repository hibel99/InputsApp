using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.Models;

public class Pivot
{
    public int ID { get; set; }
    public string pivotname { get; set; }
    public string pivotcategory { get; set; }
    public Pivot(string pivotname, string pivotcategory)
    {
        this.pivotname = pivotname;
        this.pivotcategory = pivotcategory;
    }

    public Pivot(string pivotname, string pivotcategory,int ID)
    {
        this.ID = ID;
        this.pivotname = pivotname;
        this.pivotcategory = pivotcategory;
    }



}
