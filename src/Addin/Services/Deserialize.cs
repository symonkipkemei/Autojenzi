using Autodesk.Revit.DB;
using Autojenzi.src.Addin;
using Autojenzi.src.Addin.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Resources;

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

    // Applies to Blocks only
    public double? Ratio { get; set; }
    public int? Intervals { get; set; }

}


public class MaterialList
{
    public List<MaterialJson> Materials { get; set; }
}

public static class MaterialLoader
{
    // Reads JSON file into a string variable(json) then the string variable is deserialized into a list of MaterialData object
    public static MaterialList LoadMaterials(string json)
    {
        MaterialList materials = JsonConvert.DeserializeObject<MaterialList>(json);
        return materials;
    }
    public static string LoadJsonContent(string resourcePath)
    {
        StreamResourceInfo resourceInfo = System.Windows.Application.GetResourceStream(new Uri(resourcePath));
        if (resourceInfo != null)
        {
            using (var reader = new StreamReader(resourceInfo.Stream))
            {
                return reader.ReadToEnd();
            }

        }
        else
        {
            throw new FileNotFoundException("Resource Not Found" + resourcePath);
        }
    }


    // Converts material JSON objects into a specific material object based on the class type
    public static BuildingMaterial CreateMaterial(MaterialJson materialJson)
    {
        BuildingMaterial material = new BuildingMaterial();
        material.Category = materialJson.Category;
        material.ID = materialJson.ID;
        material.Name = materialJson.Name;
        material.Rate = materialJson.Rate ?? 0;
        material.UnitOfProduct = materialJson.Unit;
        material.Density = materialJson.Density ?? 0;
        material.UnitWeight = materialJson.UnitWeight ?? 0;
        material.UnitVolume = materialJson.UnitVolume ?? 0;
        material.UnitArea = materialJson.UnitArea ?? 0;
        material.UnitLength = materialJson.UnitLength ?? 0;
        material.UnitWidth = materialJson.UnitWidth ?? 0;
        material.UnitHeight = materialJson.UnitHeight ?? 0;
        material.Thickness = materialJson.Thickness ?? 0;

        // additional properties
        material.Ratio = materialJson.Ratio ?? 0;
        material.Intervals = materialJson.Intervals ?? 0;
        
        return material;
    }

    public static BuildingMaterial FindBuildingMaterial(string id)
    {
        string resourcePath = Store.ResourcePath;
        var jsonContent = LoadJsonContent(resourcePath);
        // List of material JSON objects
        var materialData = LoadMaterials(jsonContent);

        // Convert to list of building materials
        List<BuildingMaterial> buildingMaterials = materialData.Materials.Select(m => CreateMaterial(m)).ToList();

        // Find building material with ID
        BuildingMaterial material = buildingMaterials.FirstOrDefault(m => m.ID == id);

        return material;
    }
}

