using AutoCad.Lib.Models;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Interop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoCad.Lib
{
	public class CadHelper: IDisposable
	{
		private Autodesk.AutoCAD.Interop.AcadApplication _AcadApp = default;

		#region InitializeAutocad
		private CadHelper()
		{
			// InitializeAutocad();
		}

		// Release all COM objects
		private void ReleaseComObjects()
		{
			if (_AcadApp != null && Marshal.IsComObject(_AcadApp))
				Marshal.FinalReleaseComObject(_AcadApp);

			_AcadApp = null;
		}
		public void Dispose()
		{
			ReleaseComObjects();
		}

		public bool InitializeAutocad()
		{
			ReleaseComObjects();

			// Getting running AutoCAD instance by Marshalling by passing Programmatic ID as a string, AutoCAD.Application is the Programmatic ID for AutoCAD.

			try
			{
				_AcadApp = (AcadApplication)System.Runtime.InteropServices.Marshal.GetActiveObject("AutoCAD.Application");
				_AcadApp.Visible = true;
			}
			catch (Exception ex)
			{
				throw new CadHandlingException("Can not Access Autocad Program", ex);
			}

			return true;
		}
		#endregion


		#region Instance Property
		private static CadHelper _Instance;

		public static CadHelper Instance
		{
			get
			{
				if (_Instance == null)
				{
					_Instance = new CadHelper();
				}

				return _Instance;
			}
			set { _Instance = value; }
		}
		#endregion


		private static CadArc ParseAcadArc(AcadArc arc, long parentId = 0)
		{
			return new CadArc { Id = arc.ObjectID, CenterPoint = new CadPoint((double[])arc.Center), StartPoint = new CadPoint((double[])arc.StartPoint), EndPoint = new CadPoint((double[])arc.EndPoint), AngleRadian = arc.TotalAngle, Radius = arc.Radius, StartAngleRadian = arc.StartAngle, EndAngleRadian = arc.EndAngle , ParentId = parentId };
		}

		private static CadCircle ParseAcadCircle(AcadCircle circle)
		{
			return new CadCircle { Id = circle.ObjectID, CenterPoint = new CadPoint((double[])circle.Center), Radius = circle.Radius };
		}

		private static CadLine ParseAcadLine(AcadLine line, long parentId = 0)
		{
			return new CadLine { Id = line.ObjectID, StartPoint = new CadPoint((double[])line.StartPoint), EndPoint = new CadPoint((double[])line.EndPoint) , ParentId = parentId };
		}


		private static List<CadElement> ParseAcadPlyline(AcadEntity cadPolyline)
		{
			List<CadElement> result = new List<CadElement>();
			AcadLWPolyline duplicatedLwPL = cadPolyline as AcadLWPolyline;
			AcadPolyline duplicatedPL = cadPolyline as AcadPolyline;
			Acad3DPolyline duplicated3DPL = cadPolyline as Acad3DPolyline;

			long parentId = cadPolyline.ObjectID;

			object[] explodedObjects = default;

			if (!(duplicatedLwPL is null))
			{
				explodedObjects = (object[])duplicatedLwPL.Explode();
			}
			else if (!(duplicatedPL is null))
			{
				explodedObjects = (object[])duplicatedPL.Explode();
			}

			else if (!(duplicated3DPL is null))
			{
				explodedObjects = (object[])duplicated3DPL.Explode();
			}

			foreach (var element in explodedObjects)
			{
				if (element is AcadLine)
				{
					if (Math.Round(((AcadLine)element).Length, 3) > 0)
						result.Add(CadHelper.ParseAcadLine(element as AcadLine, parentId));
				}
				else if (element is AcadArc)
				{
					if (Math.Round(((AcadArc)element).ArcLength, 3) > 0)
						result.Add(CadHelper.ParseAcadArc(element as AcadArc, parentId));
				}
			}

			foreach (var element in explodedObjects)
			{
				((AcadEntity)element).Delete();
			}

			return result;
		}



		private CadPoint CalculateAutocadPolylineCenterPointByRegion(AcadEntity cadPolyline)
		{
			if (_AcadApp is null) throw new CadHandlingException("Can not Access Autocad Program");
			
			CadPoint result = new CadPoint(0,0);

			object regionObj;
			object[] regionObjArray;
			AcadRegion region;

			AcadEntity[] polylineObj = new Autodesk.AutoCAD.Interop.Common.AcadEntity[1];



			AcadLWPolyline duplicatedLwPL = default;
			AcadPolyline duplicatedPL = default;
			double[] coordinates = new double[] { };

			// Get Coordinates
			switch (cadPolyline)
			{
				case AcadLWPolyline lwPolyline:
					coordinates = (double[])lwPolyline.Coordinates;
					result.Z = lwPolyline.Elevation;
					result.Id = lwPolyline.ObjectID;
					break;
				case AcadPolyline polyline:
					coordinates = (double[])polyline.Coordinates;
					result.Z = polyline.Elevation;
					result.Id = polyline.ObjectID;
					break;
			}

			// if have more than 2 joints polyline
			if (coordinates.Length > 4)
			{
				// Get duplicated polyline object to convert it to region
				switch (cadPolyline)
				{
					case AcadLWPolyline lwPolyline:

						duplicatedLwPL = lwPolyline.Copy();
						if (!duplicatedLwPL.Closed) duplicatedLwPL.Closed = true;
						polylineObj[0] = (AcadEntity)duplicatedLwPL;
						
						break;


					case AcadPolyline polyline:

						duplicatedPL = polyline.Copy();
						if (!duplicatedPL.Closed) duplicatedPL.Closed = true;
						polylineObj[0] = (AcadEntity)duplicatedPL;
						
						break;
				}

				regionObj = _AcadApp.ActiveDocument.ModelSpace.AddRegion(polylineObj);
				regionObjArray = (object[])regionObj;
				region = (AcadRegion)regionObjArray[0];

				double[] regionCenterCoordinates = (double[])region.Centroid;
				result.X = regionCenterCoordinates[0];
				result.Y = regionCenterCoordinates[1];
				
				region.Delete();
			}

			// if we have only 2 joints in the polyline
			else if (coordinates.Length == 4)
			{
				result.X = (coordinates[0] + coordinates[2]) * 0.5;
				result.Y = (coordinates[1] + coordinates[3]) * 0.5;
			}

			// clean duplicated polylines
			if (!(duplicatedLwPL is null)) duplicatedLwPL.Delete();
			if (!(duplicatedPL is null)) duplicatedPL.Delete();


			return result;
		}

		public List<CadPoint> GetSelected2DPolylineCenterPoints()
        {
			if (_AcadApp is null) throw new CadHandlingException("Can not Access Autocad Program");
			List<CadPoint> result = new List<CadPoint>();

			AcadSelectionSet sSet = default(AcadSelectionSet);
			string tmpString = Guid.NewGuid().ToString().Replace('-', '_');
			sSet = _AcadApp.ActiveDocument.SelectionSets.Add(tmpString);
			sSet.SelectOnScreen();
			foreach (var element in sSet)
			{
				switch (element)
				{
					case AcadLWPolyline lwPolyLine:
						result.Add(this.CalculateAutocadPolylineCenterPointByRegion(lwPolyLine as AcadEntity));
						break;

					case AcadPolyline polyline:
						result.Add(this.CalculateAutocadPolylineCenterPointByRegion(polyline as AcadEntity));
						break;
					
					default:
						break;
				}
			}
			sSet.Delete();
			return result;
		}
		public List<List<CadElement>> GetSelectedCadElements(out List<CadPoint> polylineCenterPoints )
		{
			if (_AcadApp is null) throw new CadHandlingException("Can not Access Autocad Program");
			polylineCenterPoints = new List<CadPoint>();
			List<List<CadElement>> result = new List<List<CadElement>>();
			AcadSelectionSet sSet = default(AcadSelectionSet);
			string tmpString = Guid.NewGuid().ToString().Replace('-', '_');
			sSet = _AcadApp.ActiveDocument.SelectionSets.Add(tmpString);

			sSet.SelectOnScreen();
			foreach (var element in sSet)
			{
				switch (element)
				{
					case AcadLWPolyline lwPolyLine:
						result.Add(CadHelper.ParseAcadPlyline(lwPolyLine as AcadEntity));
						polylineCenterPoints.Add(CalculateAutocadPolylineCenterPointByRegion(lwPolyLine as AcadEntity));
						break;

					case AcadPolyline polyline:
						result.Add(CadHelper.ParseAcadPlyline(polyline as AcadEntity));
						polylineCenterPoints.Add(CalculateAutocadPolylineCenterPointByRegion(polyline as AcadEntity));
						break;
					case Acad3DPolyline polyline3D:
						result.Add(CadHelper.ParseAcadPlyline(polyline3D as AcadEntity));
						break;

					case AcadCircle circle:
						result.Add(new List<CadElement> { CadHelper.ParseAcadCircle(circle) });
						break;
					case AcadLine line:
						result.Add(new List<CadElement> { CadHelper.ParseAcadLine(line) });
						break;
					case AcadArc arc:
						result.Add(new List<CadElement> { CadHelper.ParseAcadArc(arc) });
						break;

					default:
						break;
				}
			}
			sSet.Delete();
			return result;
		}
	}
}
