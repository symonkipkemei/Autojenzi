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
                var selection = new Selection();
                selection.ShowDialog();

                if (selection.IsOk)
                {
                    IList<Element> walls = Elemental.SelectMultipleWalls(uidoc, doc);
                    SumWallQuantities(walls);
                    var materialTable = new Materials(Store.AbstractedMaterials, Store.PropertiesList);
                    materialTable.ShowDialog();

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

            foreach (Element wall in walls)
            {
                abstractWall.WallLength = UnitsConversion.FootToMetre(wall.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble());
                abstractWall.WallHeight = UnitsConversion.FootToMetre(wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble());
                abstractWall.WallWidth = UnitsConversion.FootToMetre(0.656168);

                // Sum quantities /volumes for every wall
                abstractWall.BlockWall();
            }
            abstractWall.AssignQuantityAttribute();
            abstractWall.StoreData();

        }
    }
}
