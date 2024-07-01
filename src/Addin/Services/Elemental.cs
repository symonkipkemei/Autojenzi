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


        public static void Create3DView(UIDocument uidoc, Document doc) 
        {

            using (Transaction trans = new Transaction(doc, "Create autojenzi 3D view"))
            { 
                trans.Start();

                // check if the view already exists

                View3D autojenziView = new FilteredElementCollector(doc)
                    .OfClass(typeof(View3D))
                    .Cast<View3D>()
                    .FirstOrDefault(v=> v.Name.Equals("Autojenzi"));

                if (autojenziView == null)
                {
                    // Get the 3D view type in the docs
                    ViewFamilyType viewFamilyType = new FilteredElementCollector(doc)
                        .OfClass(typeof(ViewFamilyType))
                        .Cast<ViewFamilyType>()
                        .FirstOrDefault( x => x.ViewFamily == ViewFamily.ThreeDimensional);

                    if (viewFamilyType != null)
                    {
                        //Create the 3D view
                        autojenziView = View3D.CreateIsometric(doc, viewFamilyType.Id);
                        autojenziView.Name = "Autojenzi";
                    }

                }

                trans.Commit();
            
            }

            //Set created view as active

            var newView = new FilteredElementCollector(doc)
                .OfClass(typeof(View3D))
                .Cast<View3D>()
                .FirstOrDefault(v => v.Name.Equals("Autojenzi"));

            if (newView != null)
            {
                uidoc.ActiveView = newView;
            }
        }
    }
}
