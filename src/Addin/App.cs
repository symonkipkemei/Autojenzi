using Autojenzi.src.Addin.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Autojenzi.src.Addin
{
    public static class Store
    {
        // to ensure its ready for use and avoid null reference exceptions
        static Store()
        {
            AbstractedMaterials = new ObservableCollection<BuildingMaterial>();
            PropertiesList = new ObservableCollection<WallProperties>();
        }

        public static ObservableCollection<BuildingMaterial> AbstractedMaterials { get; set; }
        public static ObservableCollection<WallProperties> PropertiesList { get; set; }

        // This is the selected material by the user// add property such that if its null the program does not crash
        public static string BlockName {  get; set; }
        public static string BlockID { get; set; }
    }



}


