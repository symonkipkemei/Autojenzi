using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Autojenzi.src.Addin
{
    public class Addin : IExternalApplication
    {
        private void AddRibbonButton(UIControlledApplication application)
        {
            // Ribbon Panel
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("Autojenzi");
            string assembly = Assembly.GetExecutingAssembly().Location;


            //Button data for About command
            PushButtonData data1 = new PushButtonData("About", "About", assembly, "Autojenzi.src.Addin.Commands.AboutCommand");
            PushButton button1 = ribbonPanel.AddItem(data1) as PushButton;
            button1.ToolTip = "Info about this tool";
            Uri uri1 = new Uri("pack://application:,,,/Autojenzi;component/src/Addin/Resources/AutojenziIcon.png");
            BitmapImage image1 = new BitmapImage(uri1);
            button1.LargeImage = image1;

            //Button data for Quantify Command
            PushButtonData data = new PushButtonData("Quantify", "Quantify", assembly, "Autojenzi.src.Addin.Commands.QuantifyCommand");
            PushButton button = ribbonPanel.AddItem(data) as PushButton;
            button.ToolTip = "Abstract material metrics";
            Uri uri = new Uri("pack://application:,,,/Autojenzi;component/src/Addin/Resources/AutojenziQuantify.png");
            BitmapImage image = new BitmapImage(uri);
            button.LargeImage = image;


            
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            AddRibbonButton(application);

            return Result.Succeeded;
        }

    }
}
