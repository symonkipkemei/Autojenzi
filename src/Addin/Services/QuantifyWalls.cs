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

    internal class Quantify
    {

        //Wall Dimensional propeties (Abstracted from revit)
        double WallLength { get; set; }
        double WallHeight { get; set; }
        double WallWidth { get; set; }

        public Quantify(double wallLength, double wallHeight, double wallWidth)
        {
            WallLength = wallLength;
            WallHeight = wallHeight;
            WallWidth = wallWidth;
        }

        public void Wall()
        {
            BuildingMaterial stone = MaterialLoader.FindBuildingMaterial("MachineCutStone");

            double blockHeight = stone.UnitHeight; 
            double blockLength = stone.UnitWidth; 
            double blockWidth = stone.UnitWidth;
            double jointThickness = stone.Thickness;

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

            //blocks (full,tooth,stack)
            double fullBlocksNo = firstCourse.FullBlockNo + secondCourse.FullBlockNo;
            double stackBlocksNo = firstCourse.StackBlockNo + secondCourse.StackBlockNo;
            double toothBlockNo = firstCourse.ToothBlockNo + secondCourse.ToothBlockNo;

            stone.CalculateBlockNo(stackBlocksNo, toothBlockNo, fullBlocksNo);
          
            // Volume of joints (Horizontal + Vertical)

            double horizontalJointVolume = (firstCourse.HorizontalJointNo + secondCourse.HorizontalJointNo) * horizontalJoint.Volume;
            double verticalJointVolume = (firstCourse.VerticalJointNo +  secondCourse.VerticalJointNo) * verticalJoint.Volume;
            double totalJointVolume = verticalJointVolume * horizontalJointVolume;

            //Mortar
            Mortar mortar = new Mortar();
            mortar.MortarVolume = totalJointVolume;
            double sandVolume = mortar.SandVolume;
            double cementVolume = mortar.CementVolume;

            //Cement
            BuildingMaterial cement = MaterialLoader.FindBuildingMaterial("Cement");
            cement.TotalVolume = cementVolume;

            //Sand
            BuildingMaterial sand = MaterialLoader.FindBuildingMaterial("RiverSand");
            sand.TotalVolume = sandVolume;

            // Area of DPC Strip
            BuildingMaterial dpc = MaterialLoader.FindBuildingMaterial("DPC");
            dpc.TotalArea = dpcStrip.TotalArea;

            // Length of  Hoop Iron strip
            HoopIron hoopIron = MaterialLoader.FindBuildingMaterial("hoopIron") as HoopIron;
            hoopIron.TotalLength = hoopIronStrip.StripLength;

        }
    }
}
