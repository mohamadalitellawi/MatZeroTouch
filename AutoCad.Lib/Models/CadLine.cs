using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCad.Lib.Models
{
	public class CadLine : CadElement
	{
		public CadPoint StartPoint { get; set; }
		public CadPoint EndPoint { get; set; }
	}
}
