using System;
using System.Collections.Generic;
using System.Linq;

namespace Autojenzi.src.Addin.Services
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildingMaterial mat = MaterialLoader.FindBuildingMaterial("CMU");
    
            if (mat != null)
            {
                mat.TotalNumber = 300;
                mat.CalculateBlockVolume();
                mat.Quantity = mat.TotalNumber;
                

                Console.WriteLine($"Material: {mat.Name}, Unit: {mat.Unit}, Quantity: {mat.Quantity}");
            }
            else
            {
                Console.WriteLine("Material not found.");
            }
        }
    }
}

