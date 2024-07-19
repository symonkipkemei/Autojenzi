
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
            try
            {
                IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element, new WallSelectionFilter());

                if (references == null || references.Count == 0) 
                {
                    MessageBox.Show("No walls were selected. Please select at least one wall.", "Selection Empty", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    foreach (Reference reference in references)
                    {
                        Element wall = doc.GetElement(reference);
                        walls.Add(wall);
                    }

                }

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                MessageBox.Show("Wall selection was canceled by the user.", "Selection Canceled", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex) 
            {
                MessageBox.Show($"An error occurred during wall selection: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

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
            View3D autojenziView = null;
            try
            {
                // check if the view already exists
                autojenziView = new FilteredElementCollector(doc)
                        .OfClass(typeof(View3D))
                        .Cast<View3D>()
                        .FirstOrDefault(v => v.Name.Equals("Autojenzi"));

                if (autojenziView == null)
                {
                    using (Transaction trans = new Transaction(doc, "Create autojenzi 3D view"))
                    {
                        trans.Start();               
                        // Get the 3D view type in the docs
                        ViewFamilyType viewFamilyType = new FilteredElementCollector(doc)
                                .OfClass(typeof(ViewFamilyType))
                                .Cast<ViewFamilyType>()
                                .FirstOrDefault(x => x.ViewFamily == ViewFamily.ThreeDimensional);

                        if (viewFamilyType != null)
                            {
                                //Create the 3D view
                                autojenziView = View3D.CreateIsometric(doc, viewFamilyType.Id);
                                autojenziView.Name = "Autojenzi";
                            }                   
                        trans.Commit();
                        if (autojenziView != null)
                        {
                            uidoc.ActiveView = autojenziView;
                            //MessageBox.Show("Autojenzi 3D view created.Use this view to quantify your walls.", "3D view created", MessageBoxButton.OK, MessageBoxImage.Information);                           
                        }
                    }
                }
                else
                {
                    uidoc.ActiveView = autojenziView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while creating the 3D view: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return autojenziView;
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

        public static void OverideSelectedWallsColor( Document doc, IList<Element> walls)
        {
            Color blueColor = new Color(0, 123, 204);

            ElementId fillPatternId = GetSolidFillPatternId(doc);

            if (fillPatternId == null)
            {
                MessageBox.Show("Solid fill pattern not found in the document.");
                return;
            }

       
            OverrideGraphicSettings overrideGraphicSettings = new OverrideGraphicSettings();
            overrideGraphicSettings.SetSurfaceForegroundPatternColor(blueColor);
            overrideGraphicSettings.SetSurfaceForegroundPatternId(fillPatternId);

            using (Transaction tx = new Transaction(doc, "Showcase selected walls"))
            {
                try
                {
                    tx.Start();

                    foreach (Element wall in walls)
                    {
                        //Apply overides to each wall
                        doc.ActiveView.SetElementOverrides(wall.Id, overrideGraphicSettings);

                        //set comments parameter to selected
                        Parameter commentsParam = wall.LookupParameter("Comments");
                        if (commentsParam != null && !commentsParam.IsReadOnly)
                        {
                            commentsParam.Set("Selected");
                        }
                    }

                    tx.Commit();
                }

                catch (Exception ex) 
                {
                    string message = ex.Message;
                    string stackTrace = ex.StackTrace;

                    MessageBox.Show($"Error:{message} StackTrace {stackTrace}");
                }

            }

        }

        public static ElementId GetSolidFillPatternId(Document doc)
        {
            //Find a fill pattern Id called solid fill
          FillPatternElement solidFillPattern = new FilteredElementCollector(doc).OfClass(typeof(FillPatternElement))
                .Cast<FillPatternElement>().FirstOrDefault(fp => fp.GetFillPattern().IsSolidFill);

            return solidFillPattern?.Id;
    
        }

        public static void RemoveOveridesOnSelectedWalls( Document doc)
        {

            // select walls in revit with comments "Selected"
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Wall)).WhereElementIsNotElementType();
            var selectedWalls = from wall in collector
                                let commentsParam = wall.LookupParameter("Comments")
                                where commentsParam != null && commentsParam.AsString() == "Selected"
                                select wall;

            //Remove overides
            OverrideGraphicSettings defaultOverideSettings = new OverrideGraphicSettings();

            //Start transaction
            using (Transaction tx = new Transaction(doc, " Remove selected walls overides"))
            {
                tx.Start();

                foreach(Element wall in selectedWalls)
                {
                    doc.ActiveView.SetElementOverrides(wall.Id, defaultOverideSettings);
                }

                tx.Commit();
            }
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


