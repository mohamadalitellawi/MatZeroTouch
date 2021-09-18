using Autodesk.Revit.DB;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Revit.Elements;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wall = Autodesk.Revit.DB.Wall;

namespace MatZeroTouch
{
    public class RevitWall
    {
        private RevitWall()
        {

        }

        public static Revit.Elements.Element Create(double lengthInFt, int levelId)
        {
            Document document = DocumentManager.Instance.CurrentDBDocument;
            XYZ ptStart = new XYZ();
            XYZ ptEnd = new XYZ(lengthInFt, 0, 0);

            Line line = Line.CreateBound(ptStart, ptEnd);

            ElementId levelElementId = new ElementId(levelId);

            //Wall newWall = Wall.Create(Autodesk.Revit.DB.Document document, Autodesk.Revit.DB.Curve curve, Autodesk.Revit.DB.ElementId levelId, bool structural);

            TransactionManager.Instance.EnsureInTransaction(document);
            Wall newWall = Wall.Create(document, line, levelElementId, false);
            TransactionManager.Instance.TransactionTaskDone();

            return newWall.ToDSType(false);
        }
    }
}
