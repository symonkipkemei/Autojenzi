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

        //block/joint dimensional properties (User pre select different types of blocks)

        double BlockWidth { get { return WallWidth; } }
        double BlockHeight { get; set; }
        double BlockLength { get; set; }
        double JointThickness { get; set; }

        public Quantify(double wallLength, double wallHeight, double wallWidth, double blockHeight, double blockLength, double jointThickness)
        {
            WallLength = wallLength;
            WallHeight = wallHeight;
            WallWidth = wallWidth;
            BlockHeight = blockHeight;
            BlockLength = blockLength;
            JointThickness = jointThickness;
        }

        public void Wall()
        {
            Block fullBlock = new Block(BlockWidth, BlockLength, BlockHeight);
            Block toothBlock = new Block(BlockWidth, BlockLength, BlockHeight);
            Block stackBlock = new Block(BlockWidth, BlockLength, BlockHeight);

            Joint verticalJoint = new Joint(JointThickness,BlockHeight,BlockWidth);
            Joint horizontalJoint = new Joint(JointThickness,WallLength,BlockWidth);

            HoopIronStrip hoopIronStrip = new HoopIronStrip(WallLength);
            DpcStrip dpcStrip = new DpcStrip(WallWidth, WallLength);

            Course firstCourse = new Course(fullBlock, toothBlock, stackBlock, verticalJoint, horizontalJoint, WallLength, "First Course");
            Course secondCourse = new Course(fullBlock, toothBlock, stackBlock, verticalJoint, horizontalJoint, WallLength, "Second Course");

            //Build Blocks ( Construct blocks and joints by placing one by one to build a course)
            firstCourse.BuildBlocks(true);
            secondCourse.BuildBlocks(false);


            AbstractWall abstractWall = new AbstractWall(firstCourse, secondCourse, hoopIronStrip, dpcStrip,WallHeight);

            // Construct a wall by placing courses,hoopiron and dpc to build an abstract wall
            abstractWall.Buildcourses();


            // Abstract data//

            //No of courses (First and second course)
            double firstCourseNo = firstCourse.Number;
            double secondCourseNo = secondCourse.Number;
            double courses = firstCourseNo + secondCourseNo;

            //No of blocks (full,tooth,stack)
            double fullBlocksNo = firstCourse.FullBlockNo + secondCourse.FullBlockNo;
            double stackBlocksNo = firstCourse.StackBlockNo + secondCourse.StackBlockNo;
            double toothBlockNo = firstCourse.ToothBlockNo + secondCourse.ToothBlockNo;

            // Volume of joints (Horizontal + Vertical)

            double horizontalJointVolume = (firstCourse.HorizontalJointNo + secondCourse.HorizontalJointNo) * horizontalJoint.Volume;
            double verticalJointVolume = (firstCourse.VerticalJointNo +  secondCourse.VerticalJointNo) * verticalJoint.Volume;
            double totalJointVolume = verticalJointVolume * horizontalJointVolume;

            // Area of DPC Strip
            double dpcStripArea = dpcStrip.SectionalArea * dpcStrip.Number;

            // Length of  Hoop Iron strip
            double hoopIronStripLength = hoopIronStrip.StripLength * hoopIronStrip.Number;

            // Format data into a readable table
            Console.WriteLine("Abstracted Wall Quantities:");
            Console.WriteLine("---------------------------");
            Console.WriteLine($"Number of Courses: {courses}");
            Console.WriteLine($" - First Courses: {firstCourseNo}");
            Console.WriteLine($" - Second Courses: {secondCourseNo}");
            Console.WriteLine();
            Console.WriteLine("Number of Blocks:");
            Console.WriteLine($" - Full Blocks: {fullBlocksNo}");
            Console.WriteLine($" - Tooth Blocks: {toothBlockNo}");
            Console.WriteLine($" - Stack Blocks: {stackBlocksNo}");
            Console.WriteLine();
            Console.WriteLine("Volume of Joints:");
            Console.WriteLine($" - Horizontal Joint Volume: {horizontalJointVolume}");
            Console.WriteLine($" - Vertical Joint Volume: {verticalJointVolume}");
            Console.WriteLine($" - Total Joint Volume: {totalJointVolume}");
            Console.WriteLine();
            Console.WriteLine("DPC Strip:");
            Console.WriteLine($" - DPC Strip Area: {dpcStripArea}");
            Console.WriteLine();
            Console.WriteLine("Hoop Iron Strip:");
            Console.WriteLine($" - Hoop Iron Strip Length: {hoopIronStripLength}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Example usage
            Quantify quantify = new Quantify(10.0, 3.0, 0.2, 0.2, 0.4, 0.01);
            quantify.Wall();
        }
    }
}
