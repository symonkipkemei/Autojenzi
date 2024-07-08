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

    public class AboutCommand: IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                About about = new About();
                about.ShowDialog();

            }
            catch (Exception ex)
            {
                message = $"Message: {ex.Message}\nStackTrace: {ex.StackTrace}";
                MessageBox.Show(message);
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
}
