using Autojenzi.src.Addin.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class MaterialJson
{
    public string Category { get; set; }
    public string ID { get; set; }
    public string Name { get; set; }
    public double? Rate { get; set; }
    public string Unit { get; set; }
    public double? Density { get; set; }
    public double? UnitVolume { get; set; }
    public double? UnitWeight { get; set; }
    public double? UnitArea { get; set; }
    public double? UnitLength { get; set; }
    public double? UnitWidth { get; set; }
    public double? UnitHeight { get; set; }
    public double? Thickness { get; set; }

    public double? TotalArea { get; set; }
    public double? TotalLength { get; set; }
    public double? TotalVolume { get; set; }

}


public class MaterialList
{
    public List<MaterialJson> Materials { get; set; }
}

public static class MaterialLoader
{
    // Reads JSON file into a string variable(json) then the string variable is deserialized into a list of MaterialData object
    public static MaterialList LoadMaterials(string filepath)
    {
        string json = File.ReadAllText(filepath);
        MaterialList materials = JsonConvert.DeserializeObject<MaterialList>(json);
        return materials;
    }

    // Converts material JSON objects into a specific material object based on the class type
    public static BuildingMaterial CreateMaterial(MaterialJson materialJson)
    {
        BuildingMaterial material = new BuildingMaterial();
        material.Category = materialJson.Category;
        material.ID = materialJson.ID;
        material.Name = materialJson.Name;
        material.Rate = materialJson.Rate ?? 0;
        material.Unit = materialJson.Unit;
        material.Density = materialJson.Density ?? 0;
        material.UnitWeight = materialJson.UnitWeight ?? 0;
        material.UnitVolume = materialJson.UnitVolume ?? 0;
        material.UnitArea = materialJson.UnitArea ?? 0;
        material.UnitLength = materialJson.UnitLength ?? 0;
        material.UnitWidth = materialJson.UnitWidth ?? 0;
        material.UnitHeight = materialJson.UnitHeight ?? 0;
        material.Thickness = materialJson.Thickness ?? 0;
        return material;
    }

    public static BuildingMaterial FindBuildingMaterial(string id)
    {
        string filePath = "F:\\BIMHABITAT\\SOFTWARES\\Autojenzi\\src\\Addin\\Resources\\MaterialData.json";
        // List of material JSON objects
        var materialData = LoadMaterials(filePath);

        // Convert to list of building materials
        List<BuildingMaterial> buildingMaterials = materialData.Materials.Select(m => CreateMaterial(m)).ToList();

        // Find building material with ID
        BuildingMaterial material = buildingMaterials.FirstOrDefault(m => m.ID == id);

        return material;
    }
}

