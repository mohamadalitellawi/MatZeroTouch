using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCad.Lib.Models
{
	public class CadPoint : CadElement
	{
		public CadPoint(int id, double x, double y, double z)
		{
			Id = id; X = x; Y = y; Z = z;
		}
		public CadPoint(double x, double y) : this(-1, x, y, 0) { }
		public CadPoint(IEnumerable<double> coordinates) : this(-1, coordinates.ElementAt(0), coordinates.ElementAt(1), coordinates.ElementAt(2)) { }

		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }

		public IEnumerable<double> GetCoordinatesList()
		{
			List<double> result = new List<double> { X, Y, Z };
			return result;
		}
		public IEnumerable<double> Get2DCoordinatesList()
		{
			List<double> result = new List<double> { X, Y };
			return result;
		}
	}
}
