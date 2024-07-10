
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
using Autodesk.Revit.UI.Selection;

namespace Autojenzi.src.Addin.Services
{
    public static class Elemental
    {
        public static IList<Element> SelectMultipleWalls(UIDocument uidoc, Document doc)
        {
            IList<Element> walls = new List<Element>();
            IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element, new WallSelectionFilter());
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


        public static View3D Create3DView(UIDocument uidoc, Document doc) 
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

                    TaskDialog.Show("Revit", "Autojenzi 3D view created. Use this active view to quantify your walls");
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


            return newView;

        }

        public static void HideLevels(Document doc, View3D autojenzi)
        {
            using (Transaction trans = new Transaction(doc, "Hide Levels in 3D view"))
            {
                trans.Start();

                Category gridsCategory = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Levels);
                autojenzi.SetCategoryHidden(gridsCategory.Id, true);

                trans.Commit();
                
            }

        }

        public static void HideGrids(Document doc, View3D autojenzi)
        {
            using (Transaction trans = new Transaction(doc, "Hide Grids in 3D view"))
            {
                trans.Start();

                Category gridsCategory = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Grids);
                autojenzi.SetCategoryHidden(gridsCategory.Id, true);

                trans.Commit();

            }

        }


        public static void HideAllExceptWalls(Document doc, View3D autojenzi)
        {
            using (Transaction trans = new Transaction(doc, "Hide Everything Except walls"))
            {
                trans.Start();

                Categories categories = doc.Settings.Categories;
                
                foreach (Category category in categories)
                {
                    if (category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls)
                    {
                        continue; // skips the rest of the code
                    }
                    else if(category.get_AllowsVisibilityControl(autojenzi))
                    {
                        autojenzi.SetCategoryHidden(category.Id, true);
                    }
                }
                
                trans.Commit();

            }

        }

        public static double GetWallThickness(Wall wall )
        {
            // Get the type of wall
            WallType walltype = wall.WallType;

            // Compound structure of the wall;
            double totalThickness = 0.0;

            CompoundStructure structure = walltype.GetCompoundStructure();
            if (structure != null)
            {
                
                foreach (CompoundStructureLayer layer in structure.GetLayers())
                {
                    totalThickness += layer.Width;
                }

            }

            return totalThickness;

        }
    }


    public class WallSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            return element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}


