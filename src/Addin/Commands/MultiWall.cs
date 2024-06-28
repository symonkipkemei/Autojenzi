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
                var selection = new Selection(commandData);
                selection.ShowDialog();
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
