using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;
using AutoCad.Lib;

namespace MatZeroTouch
{
    public class DynamoCurve
    {
        private DynamoCurve()
        {

        }

        public static List<List<Curve>> GetCurvesFromAutocad2021(Vector insertShifttVector)
        {
            List<List<Curve>> result = new List<List<Curve>>();
            CadHelper.Instance.InitializeAutocad();

            var cadElements = CadHelper.Instance.GetSelectedCadElements();
            foreach (var rootElement in cadElements)
            {
                List<Curve> listDynamoElement = new List<Curve>();
                foreach (var element in rootElement)
                {
                    switch (element)
                    {
                        case AutoCad.Lib.Models.CadLine line:
                            using (Point startPoint = Point.ByCoordinates(line.StartPoint.X, line.StartPoint.Y, line.StartPoint.Z))
                            using (Point endPoint = Point.ByCoordinates(line.EndPoint.X, line.EndPoint.Y, line.EndPoint.Z))
                            {
                                listDynamoElement.Add(Line.ByStartPointEndPoint(startPoint.Add(insertShifttVector), endPoint.Add(insertShifttVector)));
                            }
                            break;
                        case AutoCad.Lib.Models.CadArc arc:
                            using (Point startPoint = Point.ByCoordinates(arc.StartPoint.X, arc.StartPoint.Y, arc.StartPoint.Z))
                            using (Point endPoint = Point.ByCoordinates(arc.EndPoint.X, arc.EndPoint.Y, arc.EndPoint.Z))
                            using (Point centerPoint = Point.ByCoordinates(arc.CenterPoint.X, arc.CenterPoint.Y, arc.CenterPoint.Z))
                            {
                                listDynamoElement.Add(Arc.ByCenterPointStartPointEndPoint(centerPoint.Add(insertShifttVector), startPoint.Add(insertShifttVector), endPoint.Add(insertShifttVector)));
                            }
                            break;
                        case AutoCad.Lib.Models.CadCircle circle:
                            using (Point centerPoint = Point.ByCoordinates(circle.CenterPoint.X, circle.CenterPoint.Y, circle.CenterPoint.Z))
                            {
                                listDynamoElement.Add(Circle.ByCenterPointRadius(centerPoint.Add(insertShifttVector), circle.Radius));
                            }
                            break;
                        default:
                            break;
                    }
                }
                result.Add(listDynamoElement);
            }
            return result;
        }
    }
}
