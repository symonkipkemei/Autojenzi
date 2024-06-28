using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autojenzi.src.Addin;
using Autojenzi.src.Addin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Autojenzi.src.UI
{
    /// <summary>
    /// Interaction logic for Selection.xaml
    /// </summary>
    public partial class Selection : Window
    {
        public List<MaterialJson> Materials { get; set; }
        public bool IsOk { get; private set; }
        // store external command data reference


        public Selection()
        {
            InitializeComponent();
            LoadMaterials();
            PopulateComboBox();
            
        }


        private void LoadMaterials()
        {
            string filePath = "F:\\BIMHABITAT\\SOFTWARES\\Autojenzi\\src\\Addin\\Resources\\MaterialData.json";
            Materials = MaterialLoader.LoadMaterials(filePath).Materials.Where(m => m.Category == "Block").ToList();

        }

        private void PopulateComboBox()
        {
           MaterialComboBox.ItemsSource = Materials.Select(m => m.Name).ToList();
        }

        private void MaterialComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedMaterialName = MaterialComboBox.SelectedItem as string;
            var selectedMaterial = Materials.FirstOrDefault(m => m.Name == selectedMaterialName);

            if (selectedMaterial != null)
            {
                var properties = new List<KeyValuePair<string, object>>()
                {
                    new KeyValuePair<string, object>("Category", selectedMaterial.Category),
                    new KeyValuePair<string, object>("Name", selectedMaterial.Name),
                    new KeyValuePair<string, object>("Rate", selectedMaterial.Rate),

                    new KeyValuePair<string, object>("UnitLength", selectedMaterial.UnitLength),
                    new KeyValuePair<string, object>("UnitWidth", selectedMaterial.UnitWidth),
                    new KeyValuePair<string, object>("UnitHeight", selectedMaterial.UnitHeight),

                    new KeyValuePair<string, object>("Mortar Thickness", selectedMaterial.Thickness),
           
                };

                PropertiesDataGrid.ItemsSource = properties;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialComboBox.SelectedValue is string selectedMaterial)
            {
               
                // Remove spacing in the string to create an ID
                string selectedMaterialID = selectedMaterial.Replace(" ", "");
                Store.BlockName = selectedMaterialID;
                IsOk = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a material.");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsOk = false;
            this.Close();
        }
    }
}
