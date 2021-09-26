using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;
using AutoCad.Lib;
using Autodesk.DesignScript.Runtime;

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

            var cadElements = CadHelper.Instance.GetSelectedCadElements(out _);
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


        [MultiReturn(new[] { "curves", "points" })]
        public static Dictionary<string,object> GetCurvesAndCenterPointFromAutocad2021(Vector insertShifttVector)
        {
            List<List<Curve>> curveResult = new List<List<Curve>>();
            List<AutoCad.Lib.Models.CadPoint> cadPoints = new List<AutoCad.Lib.Models.CadPoint>();
            List<Point> pointResult = new List<Point>();

            CadHelper.Instance.InitializeAutocad();

            List<List<AutoCad.Lib.Models.CadElement>> cadElements = CadHelper.Instance.GetSelectedCadElements(out cadPoints);

            foreach (var point in cadPoints)
            {
                pointResult.Add(Point.ByCoordinates(point.X,point.Y,point.Z));
            }

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
                curveResult.Add(listDynamoElement);
            }

            return new Dictionary<string, object> { {"curves", curveResult },{"points", pointResult } };
            
        }




        public static Line GetInclinedColumnLineFromAutocad2021(Vector insertShifttVector, double bottomLevel, double topLevel)
        {
            Line result;
            CadHelper.Instance.InitializeAutocad();

            List<AutoCad.Lib.Models.CadPoint> locationPoints  = CadHelper.Instance.GetInclinedColumnLocations();

            using (Point startPoint = Point.ByCoordinates(locationPoints[0].X, locationPoints[0].Y, bottomLevel))
            using (Point endPoint = Point.ByCoordinates(locationPoints[1].X, locationPoints[1].Y, topLevel))
            {
                result = Line.ByStartPointEndPoint(startPoint.Add(insertShifttVector), endPoint.Add(insertShifttVector));
            }

            return result;
        }



        public static List<Point> GetCurvesCenterPointsFromAutocad2021(Vector insertShifttVector, bool overrideZ = false, double z = 0)
        {
            List<Point> result = new List<Point>();
            CadHelper.Instance.InitializeAutocad();

            var cadElements = CadHelper.Instance.GetSelected2DPolylineCenterPoints();

            foreach (var p in cadElements)
            {
                result.Add(Point.ByCoordinates(p.X,p.Y, overrideZ? z: p.Z).Add(insertShifttVector));
            }
            return result;
        }
    }
}
