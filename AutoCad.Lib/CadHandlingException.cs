using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCad.Lib
{
    public class CadHandlingException : System.Exception
    {
        public CadHandlingException()
        {
        }

        public CadHandlingException(string message = "Can not Access Autocad Program")
            : base(message)
        {
        }

        public CadHandlingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
