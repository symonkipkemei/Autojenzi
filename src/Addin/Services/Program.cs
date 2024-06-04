using System;
using System.Collections.Generic;
using System.Linq;

namespace Autojenzi.src.Addin.Services
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string filePath = "F:\\BIMHABITAT\\SOFTWARES\\Autojenzi\\src\\Addin\\Resources\\MaterialData.json";

            // Load material data from JSON file
            var materialData = MaterialLoader.LoadMaterials(filePath);

            // Flatten the list of all materials
            var materials = new List<MaterialJson>();
            materials.AddRange(materialData.Materials.Block);
            materials.AddRange(materialData.Materials.Cement);
            materials.AddRange(materialData.Materials.BuildingMaterial);
            materials.AddRange(materialData.Materials.HoopIron);
            materials.AddRange(materialData.Materials.DampProofCourse);

            // Convert to list of building materials
            var buildingMaterials = materials.Select(m => MaterialLoader.CreateMaterial(m)).ToList();

            // Find a specific material by name
            var block = buildingMaterials.FirstOrDefault(m => m.Name == "Quarry Stone");

            if (block != null)
            {
                block.Quantity = 600;
                block.TotalVolume = 20;

                Console.WriteLine($"Material: {block.Name}, Unit: {block.Unit},Total Amount: {block.Amount}, Total Weight: {block.TotalWeight}");
            }
            else
            {
                Console.WriteLine("Material not found.");
            }
        }
    }
}
