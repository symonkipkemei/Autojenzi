using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Autojenzi.src.Addin.Services
{
    public class MaterialJson
    {
        public string Name { get; set; }
        public double Rate { get; set; }
        public string Unit { get; set; }
        public double Density { get; set; }
    }

    public static class MaterialLoader
    {
        // reads json file into a string variable then string variable is deserialized into a list of Materialjson objects
        public static List<MaterialJson> LoadMaterials(string filepath)
        {
            var json = File.ReadAllText(filepath);
            var materials = JsonConvert.DeserializeObject<List<MaterialJson>>(json);
            return materials;
        }

        // converts material json objects into a BuildingMaterial Object
        public static BuildingMaterial CreateBuildingMaterial(MaterialJson materialJson)
        {
            var material = new BuildingMaterial();
            material.Name = materialJson.Name;
            material.Rate = materialJson.Rate;
            material.Unit = materialJson.Unit;
            material.Density = materialJson.Density;

            return material;
        }


        public static BuildingMaterial FindBuildingMaterial(string name)
        {
            string filePath = "src\\Addin\\Resources\\MaterialData.json";
            //list of material json objects
            var materialJson = MaterialLoader.LoadMaterials(filePath);

            //list of building Materials
            var materials = materialJson.Select(m => MaterialLoader.CreateBuildingMaterial(m)).ToList();

            // find building material with name
            var block = materials.FirstOrDefault(m => m.Name == $"{name}");

            return block;
        }

    }
}

   