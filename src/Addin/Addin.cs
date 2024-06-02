﻿using System;
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
    public class App : IExternalApplication
    {
        private void AddRibbonButton(UIControlledApplication application)
        {
            // Ribbon Panel
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("Wall Selection");
            string assembly = Assembly.GetExecutingAssembly().Location;

            //Button data for Mutliwall button
            PushButtonData data = new PushButtonData("MultiWall", "MultiWall", assembly, "Autojenzi.src.Addin.Commands.MultiWall");
            PushButton button = ribbonPanel.AddItem(data) as PushButton;
            button.ToolTip = "Abstract Wall Quantities";
            Uri uri = new Uri("pack://application:,,,/Autojenzi;src/Addin/Resources/snapme.png");
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