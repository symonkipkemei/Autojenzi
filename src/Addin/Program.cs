using Autojenzi.src.Addin.Services;
using Autojenzi.src.UI;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autojenzi.src.Addin
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Display a user interface with Options to select walls
            double wallLength = 3;
            double wallHeight = 3;
            double wallThickness = 0.2;


            Application app = new Application();

            Selection selection = new Selection();
            selection.Closed += (s, e) =>
            {
                // Quantify walls after the selection window is closed
                QuantifyWalls walls = new QuantifyWalls(wallLength, wallHeight, wallThickness);

                walls.BlockWall();
                walls.AssignQuantityAttribute();
                walls.StoreData();
                walls.storeWallPropertes();

                // Second window
                Materials materialTable = new Materials(Store.AbstractedMaterials, Store.PropertiesList);
                materialTable.Show();
            };

            app.Run(selection);

        }
    }
}
