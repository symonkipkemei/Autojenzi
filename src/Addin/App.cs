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
    }



}


