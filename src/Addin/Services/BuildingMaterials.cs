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
        public string Category { get; set; }
        public string ID { get; set; }
        public string Name { get; set; } //constant
        public double Rate { get; set; } //constant
        public string Unit { get; set; } //constant
        public double Quantity { get; set; } // As per the unit of measure //dynamic
        public double Amount { get { return Quantity * Rate; } } //dynamic

        // Quantitative properties

        public double Density { get; set; } //constant
        public double UnitVolume { get; set; } //dynamic
        public double UnitWeight { get; set; } //dynamic
        public double UnitArea { get; set; } //dynamic


        public double TotalVolume { get; set; } //dynamic
        public double TotalWeight { get { return TotalVolume * Density; } } //dynamic
        public double TotalArea { get; set; }
        public double TotalNumber { get; set; }



        // Dminsional properties
        public double UnitWidth { get; set; } //constant
        public double UnitHeight { get; set; } //constant
        public double UnitLength { get; set; } //Constant
        public double Thickness { get; set; }
        public double TotalLength { get; set; }



        public double CalculateBagsNo()
        {
            double aproxBags = TotalWeight / UnitWeight;
            int EstimateBags = (int)Math.Round(aproxBags);
            return EstimateBags;
        }

        public double CalculateRolls()
        {
            double NoRolls = TotalLength / UnitLength;
            int Rolls = (int)Math.Round(NoRolls);
            return Rolls;
        }

        public double CalculateSheets()
        {
            return Math.Round(TotalArea / UnitArea);
        }

        public void CalculateBlockVolume()
        {
            UnitVolume = UnitWidth * UnitHeight * UnitLength;
            TotalVolume = UnitVolume * TotalNumber;
        }

    }
}

