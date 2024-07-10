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
    public class QuantifyCommand : IExternalCommand
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
        
                        MessageBox.Show("No walls Selected", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                    }

                    else
                    {
                        try
                        {
                            SumWallQuantities(walls);
                            var materialTable = new Materials(Store.AbstractedMaterials, Store.PropertiesList, Store.BlockName + " Wall");
                            materialTable.ShowDialog();

                        }

                        catch (InvalidOperationException ex)
                        {
                            return Result.Cancelled;
                        }

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
            QuantifyWalls quantifywalls = new QuantifyWalls();
            quantifywalls.ResetQuantities();
        


            // sum of wall properties
            double totalRunningLength = 0;
            double totalHeight = 0;
            double totalWidth = 0;
            double totalArea = 0;
            double totalVolume = 0;
            int count = 0;



            foreach (Element elem in walls)
            {
                if (elem is Wall)
                {
                    Wall wall = elem as Wall;
                    quantifywalls.WallLength = UnitsConversion.FootToMetre(wall.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble());
                    quantifywalls.WallHeight = UnitsConversion.FootToMetre(wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble());
                    quantifywalls.WallWidth = UnitsConversion.FootToMetre(Elemental.GetWallThickness(wall));
                    quantifywalls.WallArea = UnitsConversion.sqfToSqm(wall.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble());
                    quantifywalls.WallVolume = UnitsConversion.CubicFootToCubicMeter(wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble());

                }

                else if(elem is FamilyInstance)
                {
                    MessageBox.Show("Model in place wall. Parameters not established", "Terminated", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new InvalidOperationException("Model in place wall encountered, terminating the script.");

                }

                // Sum quantities /volumes for every wall
                quantifywalls.AbstractBlockQuantities();

                // Calculate the total running length, thickness,height, area and volume
                totalRunningLength += quantifywalls.WallLength;
                totalHeight += quantifywalls.WallHeight;
                totalWidth += quantifywalls.WallWidth;
                totalArea += quantifywalls.WallArea;
                totalVolume += quantifywalls.WallVolume;


                count += 1;


            }
            quantifywalls.AssignQuantityAttribute();
            quantifywalls.StoreData();

            // avarages
            double avgHeight = totalHeight / count;
            double avgWidth = totalWidth / count;
            double mortarRatio = quantifywalls.Stone.Ratio;
            double mortarThickness = quantifywalls.Stone.Thickness;

            quantifywalls.StoreWallPropertes(count,avgWidth, avgHeight, totalRunningLength, totalArea,totalVolume, mortarThickness, mortarRatio);
        }
    }
}
