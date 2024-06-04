using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autojenzi.src.Addin.Services
{

    public class Program
    {
        public static void Main(string[] args)
        {
            string filePath = "F:\\BIMHABITAT\\SOFTWARES\\Autojenzi\\src\\Addin\\Resources\\MaterialData.json";
            //list of material json objects
            var materialJson = MaterialLoader.LoadMaterials(filePath);

            //list of building Materials
            var materials = materialJson.Select(m => MaterialLoader.CreateBuildingMaterial(m)).ToList();

            // find building material with name
            var block = materials.FirstOrDefault(m => m.Name == "Block");

            block.Quantity = 600;
            block.TotalVolume = 20;

            Console.WriteLine($"Material: {block.Name}, Total Amount: {block.Amount}, Total Weight: {block.TotalWeight}");

        }
    }
    
}
