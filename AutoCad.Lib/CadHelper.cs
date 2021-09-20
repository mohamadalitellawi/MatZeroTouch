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


		private static CadArc ParseAcadArc(AcadArc arc)
		{
			return new CadArc { Id = arc.ObjectID, CenterPoint = new CadPoint((double[])arc.Center), StartPoint = new CadPoint((double[])arc.StartPoint), EndPoint = new CadPoint((double[])arc.EndPoint), AngleRadian = arc.TotalAngle, Radius = arc.Radius, StartAngleRadian = arc.StartAngle, EndAngleRadian = arc.EndAngle };
		}

		private static CadCircle ParseAcadCircle(AcadCircle circle)
		{
			return new CadCircle { Id = circle.ObjectID, CenterPoint = new CadPoint((double[])circle.Center), Radius = circle.Radius };
		}

		private static CadLine ParseAcadLine(AcadLine line)
		{
			return new CadLine { Id = line.ObjectID, StartPoint = new CadPoint((double[])line.StartPoint), EndPoint = new CadPoint((double[])line.EndPoint) };
		}


		private static List<CadElement> ParseAcadPlyline(AcadEntity cadPolyline)
		{
			List<CadElement> result = new List<CadElement>();
			AcadLWPolyline duplicatedLwPL = cadPolyline as AcadLWPolyline;
			AcadPolyline duplicatedPL = cadPolyline as AcadPolyline;
			Acad3DPolyline duplicated3DPL = cadPolyline as Acad3DPolyline;

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
						result.Add(CadHelper.ParseAcadLine(element as AcadLine));
				}
				else if (element is AcadArc)
				{
					if (Math.Round(((AcadArc)element).ArcLength, 3) > 0)
						result.Add(CadHelper.ParseAcadArc(element as AcadArc));
				}
			}

			foreach (var element in explodedObjects)
			{
				((AcadEntity)element).Delete();
			}

			return result;
		}

		public List<List<CadElement>> GetSelectedCadElements()
		{
			if (_AcadApp is null) throw new CadHandlingException("Can not Access Autocad Program");

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
						break;

					case AcadPolyline polyline:
						result.Add(CadHelper.ParseAcadPlyline(polyline as AcadEntity));
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
