using Autojenzi.src.Addin.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for Materials.xaml
    /// </summary>
    public partial class Materials : Window
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<BuildingMaterial> MaterialItems {  get; set; }
       
        public Materials(ObservableCollection<BuildingMaterial> storeitems)
        {
            InitializeComponent();
            if (storeitems == null)
            {
                MessageBox.Show("Store items cannot be null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DataContext = this;
            MaterialItems = storeitems;
        }
    }
}
