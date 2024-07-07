using Autodesk.Revit.DB.DirectContext3D;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Autojenzi.src.Addin.Services
{

    public class QuantifyWalls
    {

        //Wall Dimensional propeties (Abstracted from revit)
        public double WallLength { get; set; }
        public double WallHeight { get; set; }
        public double WallWidth { get; set; }
        public double WallArea { get; set; } //Note that the wall area is not the same as L x W because a wall be a non-orthogonal form
        public double WallVolume { get; set; }
        public BuildingMaterial Stone { get; private set; }
        public BuildingMaterial Cement { get; private set; }
        public BuildingMaterial Sand { get; private set; }
        public BuildingMaterial Dpc { get; private set; }
        public BuildingMaterial HoopIron { get; private set; }

    

        public QuantifyWalls()
        {

            // Initialize building materials
            Stone = MaterialLoader.FindBuildingMaterial(Store.BlockID);
            Cement = MaterialLoader.FindBuildingMaterial("Cement");
            Sand = MaterialLoader.FindBuildingMaterial("RiverSand");
            Dpc = MaterialLoader.FindBuildingMaterial("DPC");
            HoopIron = MaterialLoader.FindBuildingMaterial("HoopIron");

        }

        public void AbstractBlockQuantities()
        {

            Joint verticalJoint = new Joint();
            Joint horizontalJoint = new Joint();

            Block block = new Block(Stone, verticalJoint, horizontalJoint);

            //Number of Blocks & Joint volume
            double blocksFactor = WallArea / block.BlockArea;
            int blocksNo = (int)Math.Ceiling(blocksFactor);
            double totalJointVolume ;
            if (blocksFactor > 0 && blocksFactor < 1)
            {
                totalJointVolume = 0.0; //No need of mortar as block is only 1
                MessageBox.Show("No need for mortar since there's only one block", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else 
            {
                // Joint Volume
                totalJointVolume = block.BlockJointVolume * blocksNo;
            }

            

            //Mortar
            int cementRatio = (int)Math.Floor(Stone.Ratio);
            int sandRatio = (int)((Stone.Ratio - cementRatio)*10);
            Mortar mortar = new Mortar(cementRatio, sandRatio){ MortarVolume = totalJointVolume };
            double sandVolume = mortar.SandVolume;
            double cementVolume = mortar.CementVolume;


            // dpc
            DpcStrip dpcStrip = new DpcStrip(WallWidth, WallLength);    

            // Hoopiron strip
            int courses = (int)Math.Round (WallHeight/ block.BlockHeight);

            HoopIronStrip hoopIronStrip = new HoopIronStrip(WallLength, courses, Stone.Intervals);

            //Material quantities
            Stone.TotalNumber += blocksNo; //The toothblocks will be halfed on both sides depending on the number of courses
            Cement.TotalVolume += cementVolume;
            Sand.TotalVolume += sandVolume;
            Dpc.TotalArea += dpcStrip.SectionalArea;
            HoopIron.TotalLength += hoopIronStrip.TotalLength;
        }

        public void AssignQuantityAttribute()
        {
            Stone.ProductQuantity = Stone.TotalNumber;
            Cement.ProductQuantity = Cement.CalculateBagsNo();
            Sand.ProductQuantity = Math.Round(Sand.TotalWeight,4);
            Dpc.ProductQuantity = Dpc.CalculateSheets();
            HoopIron.ProductQuantity = HoopIron.CalculateRolls();
        }
        public void ResetQuantities()
        {
            Stone.ProductQuantity = 0;
            Cement.ProductQuantity = 0;
            Sand.ProductQuantity = 0;
            Dpc.ProductQuantity = 0;
            HoopIron.ProductQuantity = 0;

        }

        public void StoreData()
        {
            Store.AbstractedMaterials.Add(Stone);
            Store.AbstractedMaterials.Add(Cement);
            Store.AbstractedMaterials.Add(Sand);
            Store.AbstractedMaterials.Add(Dpc);
            Store.AbstractedMaterials.Add(HoopIron);
        }

        public void StoreWallPropertes(int count, double thickness, double height, double runningLength, double area, double volume, double mortarThickness, double mortarRatio)
        {
            Store.PropertiesList.Add(new WallProperties("Selected Elements", "Number", count));
            Store.PropertiesList.Add(new WallProperties("Thickness", "Meters (avg.)", Math.Round(thickness,2)));
            Store.PropertiesList.Add(new WallProperties("Height", "Meters (avg.)", Math.Round(height, 2)));
            Store.PropertiesList.Add(new WallProperties("Running Length", "Meters (total)", Math.Round(runningLength, 2)));
            Store.PropertiesList.Add(new WallProperties("Area", "Square Meters (total)", Math.Round(area, 2)));
            Store.PropertiesList.Add(new WallProperties("Volume", "Cubic Meters (total)", Math.Round(volume, 2)));
            Store.PropertiesList.Add(new WallProperties("Motar Thickness", "Millimeters", UnitsConversion.MtoMm(mortarThickness)));
            Store.PropertiesList.Add(new WallProperties("Motar Ratio", "cement.sand ratio", mortarRatio));
        }
    }
}
