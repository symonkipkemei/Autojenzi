using Autodesk.Revit.DB.DirectContext3D;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autojenzi.src.Addin.Services
{

    internal class QuantifyWalls
    {

        //Wall Dimensional propeties (Abstracted from revit)
        double WallLength { get; set; }
        double WallHeight { get; set; }
        double WallWidth { get; set; }


        public BuildingMaterial Stone { get; private set; }
        public BuildingMaterial Cement { get; private set; }
        public BuildingMaterial Sand { get; private set; }
        public BuildingMaterial Dpc { get; private set; }
        public BuildingMaterial HoopIron { get; private set; }

    

        public QuantifyWalls(double wallLength, double wallHeight, double wallWidth)
        {
            WallLength = wallLength;
            WallHeight = wallHeight;
            WallWidth = wallWidth;

            // Initialize building materials
            Stone = MaterialLoader.FindBuildingMaterial("MachineCutStone");
            Cement = MaterialLoader.FindBuildingMaterial("Cement");
            Sand = MaterialLoader.FindBuildingMaterial("RiverSand");
            Dpc = MaterialLoader.FindBuildingMaterial("DPC");
            HoopIron = MaterialLoader.FindBuildingMaterial("HoopIron");

        }

        public void BlockWall()
        {
            double blockHeight = Stone.UnitHeight; 
            double blockLength = Stone.UnitWidth; 
            double blockWidth = Stone.UnitWidth;
            double jointThickness = Stone.Thickness;

            BlockType fullBlock = new BlockType(blockLength, blockHeight, blockWidth);
            BlockType stackBlock = new BlockType(blockLength / (3/4), blockHeight, blockWidth);
            BlockType toothBlock = new BlockType(blockLength / (1/2), blockHeight, blockWidth);
        
            Joint verticalJoint = new Joint(jointThickness, blockHeight, blockWidth); 
            Joint horizontalJoint = new Joint(jointThickness, WallLength, blockWidth);

            HoopIronStrip hoopIronStrip = new HoopIronStrip(WallLength);
            DpcStrip dpcStrip = new DpcStrip(WallWidth, WallLength);

            Course firstCourse = new Course(fullBlock, toothBlock, stackBlock, verticalJoint, horizontalJoint, WallLength, "First Course");
            Course secondCourse = new Course(fullBlock, toothBlock, stackBlock, verticalJoint, horizontalJoint, WallLength, "Second Course");

            //Build Blocks ( Construct blocks and joints by placing one by one to build a course)
            firstCourse.BuildACourse(true);
            secondCourse.BuildACourse(false);

            WallAbstract wallAbstract = new WallAbstract(firstCourse, secondCourse, hoopIronStrip, dpcStrip,WallHeight);
            // Construct a wall by placing courses,hoopiron and dpc to build an abstract wall
            wallAbstract.BuildCourses();
            double stripNumber = wallAbstract.HoopIronStrip.Number;
            double stripLength = hoopIronStrip.StripLength;
            double totalArea = wallAbstract.HoopIronStrip.TotalLength;
            //blocks (full,tooth,stack)
            double fullBlocksNo = firstCourse.FullBlockNo + secondCourse.FullBlockNo;
            double stackBlocksNo = firstCourse.StackBlockNo + secondCourse.StackBlockNo;
            double toothBlockNo = firstCourse.ToothBlockNo + secondCourse.ToothBlockNo;

            // Volume of joints (Horizontal + Vertical)
            double horizontalJointVolume = (firstCourse.HorizontalJointNo + secondCourse.HorizontalJointNo) * horizontalJoint.Volume;
            double verticalJointVolume = (firstCourse.VerticalJointNo +  secondCourse.VerticalJointNo) * verticalJoint.Volume;
            double totalJointVolume = verticalJointVolume + horizontalJointVolume;

            //Mortar
            Mortar mortar = new Mortar();
            mortar.MortarVolume = totalJointVolume;
            double sandVolume = mortar.SandVolume;
            double cementVolume = mortar.CementVolume;

            //Material quantities
            Stone.TotalNumber += fullBlocksNo + stackBlocksNo + toothBlockNo;
            Cement.TotalVolume += cementVolume;
            Sand.TotalVolume += sandVolume;
            Dpc.TotalArea += wallAbstract.DpcStrip.TotalArea;
            double hpvalue = HoopIron.TotalLength;
            HoopIron.TotalLength += wallAbstract.HoopIronStrip.TotalLength;
        }

        public void AssignQuantityAttribute()
        {
            Stone.ProductQuantity = Stone.TotalNumber;
            Cement.ProductQuantity = Cement.CalculateBagsNo();
            Sand.ProductQuantity = Sand.TotalWeight;
            Dpc.ProductQuantity = Dpc.CalculateSheets();
            HoopIron.ProductQuantity = HoopIron.CalculateRolls();
        }
        public void ResetQuantities()
        {
            Stone.TotalNumber = 0;
            Cement.TotalVolume = 0;
            Sand.TotalVolume = 0;
            Dpc.TotalArea = 0;
            HoopIron.TotalLength = 0;
        }

        public void StoreData()
        {
            Store.AbstractedMaterials.Add(Stone);
            Store.AbstractedMaterials.Add(Cement);
            Store.AbstractedMaterials.Add(Sand);
            Store.AbstractedMaterials.Add(Dpc);
            Store.AbstractedMaterials.Add(HoopIron);
        }

        public void storeWallPropertes()
        {
            Store.PropertiesList.Add(new WallProperties("Thickness", "Millimeter", 150));
            Store.PropertiesList.Add(new WallProperties("Height", "Millimeter", 3000));
            Store.PropertiesList.Add(new WallProperties("Running Length", "Millimeter", 7600));
            Store.PropertiesList.Add(new WallProperties("Courses", "Number (avg)", 11));
            Store.PropertiesList.Add(new WallProperties("Motar Thickness", "Millimeter", 30));
            Store.PropertiesList.Add(new WallProperties("Motar Ratio", "cement.sand ratio", 1.3));
        }
    }
}
