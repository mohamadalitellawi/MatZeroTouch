using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatZeroTouch
{
    // https://github.com/DynamoDS/Dynamo/wiki/Zero-Touch-Plugin-Development

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

        /// <summary>
        /// This is an example node demonstrating how to use the Zero Touch import mechanism.
        /// It returns the input number multiplied by 2.
        /// </summary>
        /// <param name="inputNumber">Number that will get multiplied by 2</param>
        /// <returns name="outputNumber">The result of the input number multiplied by 2</returns>
        /// <search>
        /// example, multiply, math
        /// </search>
        public static double MultByTwo(double inputNumber)
        {
            return inputNumber * 2.0;
        }


        //Returning multiple values
        //The first step is to reference "DynamoServices.dll" in your project, found in the Dynamo install location, and add using Autodesk.DesignScript.Runtime; to the top of your C# script.

        [Autodesk.DesignScript.Runtime.MultiReturn(new[] { "add", "mult" })]
        public static Dictionary<string, object> ReturnMultiExample(double a, double b)
        {
            return new Dictionary<string, object>
                {
                    { "add", (a + b) },
                    { "mult", (a * b) }
                };
        }

        //Example 2: This node will display "thing one" and "thing two" in its output ports but it will show "thing 1" and "thing 2" in the node preview.
        /// <summary>
        /// The names of the output ports  
        /// match the XML returns tag only if they include descriptions.
        /// Otherwise the output ports will match the attribute names.
        /// The returned dictionary displayed in the node preview is displayed
        /// in the order of its keys as specified in the MultiReturn attribute.
        /// </summary>
        /// <returns name="thing one">first thing</returns>
        /// <returns name="thing two">second thing</returns>
        [Autodesk.DesignScript.Runtime.MultiReturn(new[] { "thing 1", "thing 2" })]
        public static Dictionary<string, List<string>> MultiReturnExample2()
        {
            return new Dictionary<string, List<string>>()
            {
                { "thing 1", new List<string>{"apple", "banana", "cat"} },
                { "thing 2", new List<string>{"Tywin", "Cersei", "Hodor"} }
            };
        }
    }
}
