using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputsApp.Models
{
	public class PivotParts
	{
		public PivotParts(string pivotCategory, string pivotPart, decimal cost, DateTime date)
		{
			PivotCategory = pivotCategory;
			PivotPart = pivotPart;
			Cost = cost;
			Date = date;
		}

		public int ID { get; set; }
		public string PivotCategory { get; set; }
		public string PivotPart { get; set; }
		public decimal Cost { get; set; }
		public DateTime Date { get; set; }
	}
}
