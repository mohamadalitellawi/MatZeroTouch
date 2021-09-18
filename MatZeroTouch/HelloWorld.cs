using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatZeroTouch
{


    public class HelloWorld
    {
        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public HelloWorld(string message)
        {
            _message = message;
        }

        public bool Contains(string Substring)
        {
            return Message.Contains(Substring);
        }
    }
}
