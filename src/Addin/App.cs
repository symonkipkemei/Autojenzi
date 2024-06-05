using Autojenzi.src.Addin.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Autojenzi
{
    public static class Store
    {
        // to ensure its ready for use and avoid null reference exceptions
        static Store()
        {
            AbstractedMaterials  = new ObservableCollection<BuildingMaterial>();
        }

        public static ObservableCollection<BuildingMaterial> AbstractedMaterials {  get; set; }
    }


}