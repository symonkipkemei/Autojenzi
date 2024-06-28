﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB.Structure;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Autojenzi.src.Addin.Services
{
    public static class Elemental
    {
        public static IList<Element> SelectMultipleWalls(UIDocument uidoc, Document doc)
        {
            IList<Element> walls = new List<Element>();
            IList<Reference> references = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element);
            foreach (Reference reference in references)
            {
                Element wall = doc.GetElement(reference);
                walls.Add(wall);
            }

            return walls;
        }

        public static IList<Element> SelectAllWalls(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<Element> walls = collector.OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().ToElements();
            return walls;
        }

        public static void SumWallQuantities(IList<Element> walls)
        {
            // Instantiate materials to be used
            QuantifyWalls abstractWall = new QuantifyWalls();

            foreach (Element wall in walls) 
            {
                abstractWall.WallLength = UnitsConversion.FootToMetre(wall.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble());
                abstractWall.WallHeight = UnitsConversion.FootToMetre(wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble());
                abstractWall.WallWidth = UnitsConversion.FootToMetre(0.656168);

                abstractWall.BlockWall();
               
            }

            abstractWall.AssignQuantityAttribute();
            abstractWall.StoreData();
        }

    }
}
