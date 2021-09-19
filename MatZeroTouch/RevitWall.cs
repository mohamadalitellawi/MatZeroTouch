﻿using Autodesk.Revit.DB;
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

            // A transaction is a context required in Revit in order to make any 
            // modifications to a model – this includes the creation or deletion of elements
            // Without a transaction in out method would throw an exception as we are attempting to modify the document.

            TransactionManager.Instance.EnsureInTransaction(document);
            Wall newWall = Wall.Create(document, line, levelElementId, false);
            TransactionManager.Instance.TransactionTaskDone();

            /*
             * Tip: It is possible to use the Revit API’s Transaction class for the same purpose, however it is 
                    recommended to use the TransactionManager class from Dynamo’s RevitServices library as It 
                    saves you the effort of having to dispose your objects (aka garbage collection), plus it helps to 
                    keep both Revit and Dynamo in more consistent states. 
             */

            //you'll need to manually manage the geometry resources created in your functions which are not returned out of your functions.
            //Any resources returned out of your functions will be managed by the Dynamo engine.
            document.Dispose();line.Dispose();




            /*
             *  to make the Revit wall 
                element Dynamo-compatible: we need to wrap it in Dynamo’s Revit.Elements.Element wrapper 
                class. This library is an interop library which delivers all of Dynamo’s Revit nodes you see in its 
                node library, as well as helper functions to wrap and unwrap elements from Revit.
             * The method we call to ‘wrap’ Revit elements into the Dynamo wrapper class is ToDSType(). It 
                takes a bool input, where true is used if the element exists in Revit, or false if the element is 
                being instantiated by our code
             * */
            return newWall.ToDSType(false);
        }
    }
}
