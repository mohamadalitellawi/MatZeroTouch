using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCad.Lib.Models
{
	public class CadArc : CadElement
	{
		public double Radius { get; set; }
		public CadPoint CenterPoint { get; set; }
		public CadPoint StartPoint { get; set; }
		public CadPoint EndPoint { get; set; }
		public double AngleRadian { get; set; }
		public double StartAngleRadian { get; set; }
		public double EndAngleRadian { get; set; }
	}
}
