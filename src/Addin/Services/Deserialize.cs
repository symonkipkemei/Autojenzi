using Autojenzi.src.Addin.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class MaterialJson
{
    public string Class { get; set; }
    public string ID { get; set; }
    public string Name { get; set; }
    public double? Rate { get; set; }
    public string Unit { get; set; }
    public double? Density { get; set; }
    public double? UnitVolume { get; set; }
    public double? UnitWeight { get; set; }
    public double? UnitLength { get; set; }
    public double? UnitWidth { get; set; }
    public double? UnitHeight { get; set; }
    public double? JointThickness { get; set; }
}

public class MaterialCategory
{
    public List<MaterialJson> Block { get; set; }
    public List<MaterialJson> Cement { get; set; }
    public List<MaterialJson> BuildingMaterial { get; set; }
    public List<MaterialJson> HoopIron { get; set; }
    public List<MaterialJson> DampProofCourse { get; set; }
}

public class MaterialData
{
    public MaterialCategory Materials { get; set; }
}

public static class MaterialLoader
{
    // Reads JSON file into a string variable then the string variable is deserialized into MaterialData object
    public static MaterialData LoadMaterials(string filepath)
    {
        string json = File.ReadAllText(filepath);
        MaterialData materials = JsonConvert.DeserializeObject<MaterialData>(json);
        return materials;
    }

    // Converts material JSON objects into a specific material object based on the class type
    public static BuildingMaterial CreateMaterial(MaterialJson materialJson)
    {
        BuildingMaterial material;

        switch (materialJson.Class)
        {
            case "Block":
                material = new Block
                {
                    UnitWidth = materialJson.UnitWidth ?? 0,
                    UnitHeight = materialJson.UnitHeight ?? 0,
                    UnitLength = materialJson.UnitLength ?? 0,
                    JointThickness = materialJson.JointThickness ?? 0
                };
                break;

            case "Cement":
                material = new Cement();
                break;

            case "HoopIron":
                material = new HoopIron
                {
                    UnitLength = materialJson.UnitLength ?? 0,
                    Guage = materialJson.UnitWeight ?? 0
                };
                break;

            case "DampProofCourse":
                material = new DampProofCourse
                {
                    UnitWidth = materialJson.UnitWidth ?? 0,
                    UnitLength = materialJson.UnitLength ?? 0
                };
                break;

            default:
                material = new BuildingMaterial();
                break;
        }

        material.Class = materialJson.Class;
        material.Name = materialJson.Name;
        material.Rate = materialJson.Rate ?? 0;
        material.Unit = materialJson.Unit;
        material.Density = materialJson.Density ?? 0;
        material.UnitWeight = materialJson.UnitWeight ?? 0;

        return material;
    }

    public static BuildingMaterial FindBuildingMaterial(string name)
    {
        string filePath = "F:\\BIMHABITAT\\SOFTWARES\\Autojenzi\\src\\Addin\\Resources\\MaterialData.json";
        // List of material JSON objects
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

        // Find building material with name
        var block = buildingMaterials.FirstOrDefault(m => m.ID == name);

        return block;
    }
}
