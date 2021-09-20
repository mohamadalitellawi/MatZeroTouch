using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCad.Lib.Models
{
	public class CadCircle : CadElement
	{

		public CadPoint CenterPoint { get; set; }
		public double Radius { get; set; }
	}
}
