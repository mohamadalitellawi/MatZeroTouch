using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCad.Lib.Models
{
    public class CadElement
    {
        public long Id { get; set; }

        // this property to control polyline children
        public long ParentId { get; set; }
    }
}
