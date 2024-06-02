using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Autojenzi.src.Addin.Services
{
    internal class DpcStrip : DampProofCourse
    {
        // Dimensional Properties
        public double SectionWidth { get; set; }
        public double SectionLength { get; set; }

        public double SectionalArea { get {  return SectionLength * SectionWidth; } }
        public int Number { get; set; }

        internal DpcStrip(double WallWidth, double WallLength)
        {
            double offset = 25; //25mm
            SectionWidth = offset + WallWidth + offset;
            SectionLength = WallLength;
        }
    }


    internal class HoopIronStrip : HoopIron
    {
        public double StripLength { get; set; }
        public int Number { get; set; }

        internal HoopIronStrip(double WallLength)
        {
            StripLength = WallLength;
        }

    }
    internal class Mortar
    {

        public int RatioSand { get; set; }
        public int RatioCement { get; set; }
        public int TotalRatio { get { return RatioCement + RatioSand; } }
        public double MortarVolume { get; set; }

        public double CementVolume
        {
            get
            {
                double ratio = (double)RatioCement / TotalRatio;
                double cementVolume = ratio * MortarVolume;
                return cementVolume;
            }
        }

        public double SandVolume
        {
            get
            {
                double ratio = (double) RatioSand / TotalRatio;
                double sandVolume = ratio * MortarVolume;
                return sandVolume;
            }
        }

        internal Mortar(int ratioCement = 1, int ratioSand = 4)
        {
            RatioSand = ratioSand; RatioCement = ratioCement;
        }
    }


    internal class Joint
    {
        //Dimensional Properties
        public double Thickness { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public int Number { get; set; }
        public double Volume { get { return Thickness * Length * Width; } }

        public Joint(double thickness, double length, double width)
        {
            Thickness = thickness; Length = length; Width = width;
        }
    }


    internal class Block
    {
        // Dimensional properties
        public double Width { get; set; }
        public double Height { get; set; }
        public double Length { get; set; }
        public int Number {  get; set; }

        public double Volume { get { return Width * Height * Length; } }

        public Block(double width,double length , double height)
        {
            Width = width;  Length = length; Height = height;
        }

    }

    internal class Course
    {
        // Material Properties/Objects
        public Block FullBlock { get; set; }
        public Block ToothBlock { get; set; }
        public Block StackBlock { get; set; }
        public Joint VerticalJoint { get; set; }
        public Joint HorizontalJoint { get; set; }

        //Descriptive Properties
        public double CourseLength { get; set; }
        public double CourseHeight { get { return FullBlock.Height + HorizontalJoint.Thickness; } }
        public string CourseName { get; set; }
        public double TrackLength { get; set; } //Reduces until it diminishes to 0 ( Used to track How many elements are needed in a complete course)

        public double Number { get; set; } //Number of courses

        // Total Quantities of blocks/joints in this course
        public double FullBlockNo { get { return FullBlock.Number * Number; } }
        public double ToothBlockNo { get { return ToothBlock.Number * Number; } }
        public double StackBlockNo { get { return StackBlock.Number * Number; } }

        public double HorizontalJointNo { get { return HorizontalJoint.Number * Number; } }
        public double VerticalJointNo { get { return VerticalJoint.Number * Number; } }

        public Course(Block fullblock, Block toothBlock, Block stackBlock, Joint veriticalJoint, Joint horizontalJoint, double wallLength, string name)
        {
            FullBlock = fullblock; ToothBlock = toothBlock; StackBlock = stackBlock; VerticalJoint = veriticalJoint;
            HorizontalJoint = horizontalJoint; CourseLength = wallLength; TrackLength = wallLength; CourseName = name;
        }


        public void BuildBlocks(bool firstCourse)
        {
            HorizontalJoint.Number += 1;

            // start with corner stones
            if (firstCourse)
            {
                // add full stone first
                FullBlock.Number += 1; //at the start
                VerticalJoint.Number += 1; // after first course
                FullBlock.Number += 1; // at the end

                //substract the lengthd to now what's remaining
                TrackLength = CourseLength - (FullBlock.Length + VerticalJoint.Thickness + FullBlock.Length);

            }

            else
            {
                // add tooth stone
                ToothBlock.Number += 1; //at the start
                VerticalJoint.Number += 1; // after first course
                ToothBlock.Number += 1; //at the end

                TrackLength = CourseLength - (ToothBlock.Length + VerticalJoint.Thickness + ToothBlock.Length);

            }


            while (TrackLength > 0)
            {
                // if length is less than half of a toothblock length
                if (TrackLength < (ToothBlock.Length / 2))
                {
                    // add mortart size
                    VerticalJoint.Number += 1;
                    TrackLength -= VerticalJoint.Thickness;
                }

                else if (TrackLength >= (ToothBlock.Length / 2) && TrackLength < (FullBlock.Length + VerticalJoint.Thickness))
                {
                    // add stack block
                    StackBlock.Number += 1;
                    TrackLength -= StackBlock.Length; // program terminates
                }

                else
                {
                    //insert block and mortar
                    FullBlock.Number += 1;
                    VerticalJoint.Number += 1;

                    // adjust new dimensions of built masonry
                    TrackLength -= FullBlock.Length;
                    TrackLength -= VerticalJoint.Thickness;

                }
            }

        }
    }




    internal class AbstractWall
    {
        public Course FirstCourse { get; set; }
        public Course SecondCourse { get; set; }
        public HoopIronStrip HoopIronStrip { get; set; }
        public DpcStrip DpcStrip { get; set; }
 
        public double WallHeight { get; set; }

        internal AbstractWall(Course firstCourse, Course secondCourse, HoopIronStrip hoopIronStrip, DpcStrip dpcStrip, double wallHeight)
        {

            // store objects into abstract wall properties
            FirstCourse = firstCourse;
            SecondCourse = secondCourse;
            HoopIronStrip = hoopIronStrip;
            DpcStrip = dpcStrip;
            WallHeight = wallHeight;
        }

        public void Buildcourses()
        {

            // insert DPC
            DpcStrip.Number += 1;


            int counter = 0;
            double trackHeight = WallHeight; //Adjusts to 0

            while (trackHeight > 0)
            {
                // add counter to determine between first and alternate course
                counter++;

                if (counter % 2 != 0) // this is a first course
                {
                    if (trackHeight >= FirstCourse.CourseHeight)
                    {
                        FirstCourse.Number += 1;
                        trackHeight -= FirstCourse.CourseHeight;
                    }

                    else if (trackHeight < FirstCourse.CourseHeight && trackHeight > 0)
                    {
                        // This course will be less than the full course and the final course
                        FirstCourse.Number += 1;
                        trackHeight -= FirstCourse.CourseHeight;
                    }
                }

                else // This is the second course
                {
                    if (trackHeight >= SecondCourse.CourseHeight)
                    {
                        SecondCourse.Number += 1;
                        HoopIronStrip.Number += 1;
                        trackHeight -= SecondCourse.CourseHeight;

                    }

                    else if (trackHeight < SecondCourse.CourseHeight && trackHeight > 0)
                    {
                        // This course will be less than the full course and the final course
                        SecondCourse.Number += 1;
                        HoopIronStrip.Number += 1;
                        trackHeight -= SecondCourse.CourseHeight;
                    }
                }

            }
            // Terminates when track height equites to 0 or negative
        }

    }



}
