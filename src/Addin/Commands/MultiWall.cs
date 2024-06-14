using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autojenzi.src.Addin.Services;
using Autojenzi.src.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace Autojenzi.src.Addin.Commands
{
    // Attributes
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class MultiWall: IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // Display a user interface with Options to select walls
                double wallLength = 3;
                double wallHeight = 3;
                double wallThickness = 0.2;

                QuantifyWalls walls = new QuantifyWalls(wallLength, wallHeight, wallThickness);

                walls.BlockWall();
                walls.AssignQuantityAttribute();
                walls.StoreData();
                

                
                Materials materialTable = new Materials(Store.AbstractedMaterials, Store.PropertiesList);
                materialTable.ShowDialog();

            }

            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;

        }

    }
}
