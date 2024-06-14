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

            QuantifyWalls walls = new QuantifyWalls(wallLength, wallHeight, wallThickness);

            walls.BlockWall();
            walls.AssignQuantityAttribute();
            walls.StoreData();
            walls.storeWallPropertes();

            Materials materialTable = new Materials(Store.AbstractedMaterials, Store.PropertiesList);

            Application app = new Application();
            app.Run(materialTable);
           
        }
    }
}
