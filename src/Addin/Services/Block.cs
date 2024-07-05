using Autodesk.Revit.DB.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autojenzi.src.Addin.Services
{
    public class Block
    {
        //Objects required within a block
        public BuildingMaterial Stone { get; private set; } //Orthogonal block
        public Joint HorizontalJoint { get; private set; }
        public Joint VerticalJoint { get; private set; }


        // Block Parameters
        public  double BlockHeight{ get; private set; }
        public double BlockWidth{ get; private set; } //Thickness
        public double BlockLength { get; private set; }
        public double BlockArea { get { return BlockLength * BlockHeight; } }
        public double BlockJointVolume { get { return HorizontalJoint.Volume + VerticalJoint.Volume; } }

        public Block( BuildingMaterial stone, Joint horizontalJoint, Joint verticalJoint ) 
        { 
            Stone = stone ?? throw new ArgumentNullException(nameof(stone)); ;
            HorizontalJoint = horizontalJoint ?? throw new ArgumentNullException(nameof(horizontalJoint)); ;
            VerticalJoint = verticalJoint ?? throw new ArgumentNullException(nameof(verticalJoint)); ;


            BlockHeight = stone.UnitHeight  + stone.Thickness;
            BlockLength = stone.UnitLength + stone.Thickness;
            BlockWidth = stone.UnitWidth; //can be wall Width/stone width

            HorizontalJoint.Length = stone.UnitLength + stone.Thickness;
            HorizontalJoint.Height = stone.Thickness;
            HorizontalJoint.Width = stone.UnitWidth; //can be wall Width/stone width

            VerticalJoint.Width = stone.UnitWidth; //can be wall Width/stone width
            VerticalJoint.Height = stone.UnitHeight + stone.Thickness;
            VerticalJoint.Length = stone.Thickness;

        }

    }

    public class Joint
    {
        //Dimensional Properties
        public double Width { get; set; } //Thickness
        public double Length { get; set; }
        public double Height { get; set; }
        public int Number { get; set; }
        public double Volume { get { return Height * Length * Width; } }

    }
    public class Mortar
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
                double ratio = (double)RatioSand / TotalRatio;
                double sandVolume = ratio * MortarVolume;
                return sandVolume;
            }
        }

        public Mortar(int ratioCement = 1, int ratioSand = 3)
        {
            RatioSand = ratioSand; RatioCement = ratioCement;
        }
    }

    public class DpcStrip
    {
        // Dimensional Properties
        public double SectionWidth { get; set; }
        public double SectionLength { get; set; }
        public double SectionalArea { get { return SectionLength * SectionWidth; } }

        internal DpcStrip(double WallWidth, double WallLength)
        {
            double offset = 25; //25mm
            SectionWidth = offset + WallWidth + offset;
            SectionLength = WallLength;
        }
    }

    internal class HoopIronStrip
    {
        public double StripLength { get; set; }
        public int Intervals { get; set; }
        public double TotalLength { get { return StripLength * Intervals; } }

        internal HoopIronStrip(double WallLength, int courses, int spacing)
        {
            Intervals = (courses / spacing);
            StripLength = WallLength;
        }

    }
}
