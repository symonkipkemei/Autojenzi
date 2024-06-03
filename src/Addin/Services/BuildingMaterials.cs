using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autojenzi.src.Addin.Services
{
    internal class Stone
    {
        // Descriptive Properties
        public string Name { get; set; }
        public string WeightUnit { get; set; }
        public string NumberUnit { get; set; }
        public int Quantity { get; set; }
        public double Rate { get; set; }

        public double Density { get; set; }

        public double Amount { get { return Quantity * Rate; } }

        internal Stone(string name = "Machine Cut Stone",
                      string unit = "Pieces",
                      string weightUnit = "tn",
                      double rate = 50,
                      double density = 0.001)
        {
            Name = name; WeightUnit = weightUnit; NumberUnit = unit; Rate = rate; Density = density;
        }
    }


    internal class Sand
    {
        // Descriptive Properties
        public string Name { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public double Rate { get; set; }
        public double Density { get; set; }

        // Dimensional properties
        public double Volume { get; set; }


        // Read only Properties
        public double Amount { get { return Quantity * Rate; } }
        public double Weight { get { return Volume * Density; } } //in grams

        internal Sand(string name = "River Sand",
                      string unit = "Kilograms (kg)",
                      double rate = 300,
                      double density = 0.001602) //g/mm3 ( 1602kg/m3)
        {
            Name = name; Unit = unit; Rate = rate; Density = density;
        }
    }


    internal class Cement
    {
        // Descriptive Properties
        public string Name { get; set; }
        public string Unit { get; set; }
        public double Rate { get; set; }
        public double Density { get; set; }

        public double BagQuantity { get; set; } //How many Kgs in one bag of cement


        // Dimensional properties
        public double Volume { get; set; } //in M3


        // Read Only Properties
        public double Amount { get { return Quantity * Rate; } }
        public double Weight { get { return Volume * Density; } }

        public double Quantity 
        { 
            get {
                double aproxBags = (Volume * Density) / BagQuantity;
                int EstimateBags = (int)Math.Round(aproxBags);
                return EstimateBags;
                } 
        } //No of bags required

        internal Cement(string name = "Bamburi Cement",
                        string unit = "Kilograms (Kg)",
                        double rate = 605,
                        double density = 1440, // kg/m3
                        double bagQuantity = 50)
        {
            Name = name; Unit = unit; Rate = rate; Density = density; BagQuantity = bagQuantity;
  
        }

       
    }


    internal class HoopIron
    {
        // Descriptive Properties

        public string UnitName { get; set; }
        public string UnitMeasure { get; set; }
        public double UnitWeight { get; set; }
        public double UnitLength { get; set; }
        public double UnitRate { get; set; }
        public double UnitGuage { get; set; }
        public double UnitDensity { get; set; }
        // Dimensional Properties

        public double InputLength { get; set; }

        //Read Only Properties

        public double OutputWeight { get { return (InputLength * UnitWeight) / UnitLength; }} // in Kg

        public double Rolls 
        { get 
            {
                double NoRolls = InputLength / UnitLength;
                int Rolls = (int)Math.Round(NoRolls);
                return Rolls; 
            } 
        }

        public double Amount { get { return Rolls * UnitRate; } }

        //Construtor
        internal HoopIron
            (
            double length = 20, // 20m ( In Meters)
            string name= "Hoop Iron (16 Guage)",
            string unit = "Rolls",
            double rate = 3500,
            double weight = 20000,//20kg (In Kg)
            double guage = 16)
        {
            UnitLength = length; UnitName = name; UnitMeasure = unit; UnitRate = rate; UnitGuage = guage; UnitWeight = weight;
        }
    }


    internal class DampProofCourse
    {
        // Descriptive Properties

        public string Name { get; set; }
        public string Unit { get; set; }
        public double Quantity { get; set; }
        public double Rate { get; set; }


        // Dimensional properties
        public double UnitWidth { get; set; }
        public double UnitLength { get; set; }
        public double UnitArea { get { return UnitWidth * UnitLength; } }

        public double sectionalArea {  get; set; }

        public double rolls { get { return Math.Round(sectionalArea/UnitArea); } }

        internal DampProofCourse(string name = "Damp Proof Course (DPC)", string unit = "Rolls", double rate = 2200, double unitWidth = 1, double unitLength = 7)
        {
            Name = name; Unit = unit; Rate = rate; UnitWidth = unitWidth; UnitLength = unitLength;
        }


        public double Amount()
        {
            return Quantity * Rate;
        }

    }



}
