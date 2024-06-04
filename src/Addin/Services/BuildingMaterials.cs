using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autojenzi.src.Addin.Services
{
    public class BuildingMaterial
    {
        //Descriptive properties 
        public string Type { get; set; }
        public string Name { get; set; } //constant
        public double Rate { get; set; } //constant
        public string Unit { get; set; } //constant
        public double Quantity { get; set; } // As per the unit of measure //dynamic
        public double Amount { get { return Quantity * Rate; } } //dynamic

        // Quantitative properties

        public double Density { get; set; } //constant
        public double UnitVolume { get; set; } //dynamic
        public double UnitWeight { get; set; } //dynamic

        public double TotalVolume { get; set; } //dynamic
        public double TotalWeight { get { return TotalVolume * Density; } } //dynamic

    }

    internal class Block : BuildingMaterial
    {
        // Dimensional properties
        public double UnitWidth { get; set; } //constant
        public double UnitHeight { get; set; } //constant
        public double UnitLength { get; set; } //Constant
        public double UnitVolume { get { return UnitWidth * UnitHeight * UnitLength; } }


        public double StackBlockNo {  get; set; }
        public double ToothBlockNo { get; set; }
        public double FullBlockNo { get; set; }
        public double JointThickness {  get; set; }

        public double TotalNumber { get { return StackBlockNo + ToothBlockNo + FullBlockNo; } }
        public double TotalVolume { get { return TotalNumber * UnitVolume; } }
    }

    internal class Cement: BuildingMaterial
    {
        public int BagsNo 
        { 
            get {
                double aproxBags = TotalWeight / UnitWeight;
                int EstimateBags = (int)Math.Round(aproxBags);
                return EstimateBags;
                } 
        } 

    }


    internal class HoopIron: BuildingMaterial
    {
        public double UnitLength { get; set; } //Length of a roll
        public double Guage { get; set; }
        public double TotalLength { get; set; } 
        public double TotalWeight { get { return (TotalLength * UnitWeight) / UnitLength; }} // in Kg

        public double Rolls 
        { get 
            {
                double NoRolls = TotalLength / UnitLength;
                int Rolls = (int)Math.Round(NoRolls);
                return Rolls; 
            } 
        }
    }


    internal class DampProofCourse:BuildingMaterial
    {

        public double UnitWidth { get; set; }
        public double UnitLength { get; set; }
        public double UnitArea { get { return UnitWidth * UnitLength; } }
        public double TotalArea {  get; set; }
        public double rolls { get { return Math.Round(TotalArea / UnitArea); } }

    }


}
