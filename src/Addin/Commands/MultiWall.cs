using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autojenzi.src.Addin.Services;
using Autojenzi.src.UI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace Autojenzi.src.Addin.Commands
{
    // Attributes
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MultiWall : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // Display a user interface with Options to select building technology
                View3D autojenziView = Elemental.Create3DView(uidoc, doc);
                
                Elemental.HideLevels(doc, autojenziView);
                Elemental.HideGrids(doc, autojenziView);
                Elemental.HideAllExceptWalls(doc, autojenziView);

                // Add a timeout
                var selection = new Selection();
                selection.ShowDialog();

                if (selection.IsOk)
                {
                    // Ensure the user has selected walls before proceeding

                    IList<Element> walls = Elemental.SelectMultipleWalls(uidoc, doc);
                    if (walls.Count == 0)
                    {
                        TaskDialog.Show("Error", "No walls Selected ");
                    
                    }

                    else
                    {
                        SumWallQuantities(walls);
                        var materialTable = new Materials(Store.AbstractedMaterials, Store.PropertiesList, Store.BlockName + " Wall");
                        materialTable.ShowDialog();

                    }
                    

                }

                else
                {
                    return Result.Cancelled;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }

        public static void SumWallQuantities(IList<Element> walls)
        {
            // Instantiate materials to be used
            QuantifyWalls abstractWall = new QuantifyWalls();


            // sum of wall properties
            double totalRunningLength = 0;
            double totalHeight = 0;
            double totalWidth = 0;
            double totalCourses = 0;
            double count = 0;



            foreach (Element elem in walls)
            {
                Wall wall = elem as Wall;

                abstractWall.WallLength = UnitsConversion.FootToMetre(wall.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble());
                abstractWall.WallHeight = UnitsConversion.FootToMetre(wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble());
                abstractWall.WallWidth = UnitsConversion.FootToMetre(Elemental.GetWallThickness(wall));

                // Sum quantities /volumes for every wall
                abstractWall.BlockWall();

                // Calculate the total running length, thickness, and height
                totalRunningLength += abstractWall.WallLength;
                totalHeight += abstractWall.WallHeight;
                totalWidth += abstractWall.WallWidth;
                totalCourses += abstractWall.TotalCourseNo;
                count += 1;

            }
            abstractWall.AssignQuantityAttribute();
            abstractWall.StoreData();

            // avarages
            double avgHeight = totalHeight / count;
            double avgWidth = totalWidth / count;
            double avgCourses = totalCourses / count;
            double mortarRatio = abstractWall.Stone.Ratio;
            double mortarThickness = abstractWall.Stone.Thickness;

            abstractWall.storeWallPropertes(avgWidth, avgHeight, totalRunningLength, avgCourses, mortarThickness, mortarRatio);
        }
    }
}
