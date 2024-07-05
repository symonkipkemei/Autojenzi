using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Autojenzi.src.Addin.Services
{
    public class BuildingMaterial
    {
        //Descriptive properties 
        public string Category { get; set; }
        public string ID { get; set; }
        public string Name { get; set; } //constant
        public double Rate { get; set; } //constant
        public string UnitOfProduct { get; set; } //constant
        public double ProductQuantity { get; set; } // As per the unit of measure //dynamic
        public double Amount { get { return Math.Round(ProductQuantity * Rate); } } //dynamic

        // Quantitative properties

        public double Density { get; set; } //constant
        public double UnitVolume { get; set; } //dynamic
        public double UnitWeight { get; set; } //dynamic
        public double UnitArea { get; set; } //dynamic


        public double TotalVolume { get; set; } //dynamic
        public double TotalWeight { get { return TotalVolume * Density; } } //dynamic
        public double TotalArea { get; set; }
        public double TotalNumber { get; set; }

        public double TotalLength { get; set; }

        // Dimensional properties
        public double UnitWidth { get; set; } //constant
        public double UnitHeight { get; set; } //constant
        public double UnitLength { get; set; } //Constant
        
        

        //Additional properties

        public double Ratio { get; set; } // Mortar Ratio
        public int Intervals { get; set; } //Interpreted as Hoop iron intervals
        public double Thickness { get; set; } //Mortar Thickness



        public double CalculateBagsNo()
        {
            double aproxBags = TotalWeight / UnitWeight;
            int EstimateBags = (int)Math.Round(aproxBags);
            return EstimateBags;
        }

        public double CalculateRolls()
        {
            double noRolls = TotalLength / UnitLength;
            int rolls = (int)Math.Round(noRolls);
            if (rolls < 1) { rolls =1; }
            return rolls;
        }

        public double CalculateSheets()
        {
            double noSheets = TotalArea / UnitArea;
            int sheets = (int) Math.Round(noSheets);
            if (sheets < 1) { sheets = 1; }
            return sheets;
        }

        public void CalculateBlockVolume()
        {
            UnitVolume = UnitWidth * UnitHeight * UnitLength;
            TotalVolume = UnitVolume * TotalNumber;
        }

    }

    public class MaterialItemExport
    {
        public string Name { get; set; }
        public string UnitOfProduct { get; set; }
        public double ProductQuantity { get; set; }
        public double Rate { get; set; }
        public double Amount { get; set; }
    }

}

