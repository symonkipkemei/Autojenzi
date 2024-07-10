using Autojenzi.src.Addin.Services;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using Microsoft.Win32;
using OfficeOpenXml;
using System.Diagnostics;

namespace Autojenzi.src.UI
{
    /// <summary>
    /// Interaction logic for Materials.xaml
    /// </summary>
    public partial class Materials : Window
    {
  
        public ObservableCollection<BuildingMaterial> MaterialItems {  get; set; }

        public ObservableCollection<WallProperties> WallProperties { get; set; }

 
        public Materials(ObservableCollection<BuildingMaterial> storeitems, ObservableCollection<WallProperties> wallProperties, string titleName)
        {
            InitializeComponent();
            if (storeitems == null)
            {
                MessageBox.Show("Store items cannot be null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DataContext = this;
            tbTitle.Text = titleName;
            MaterialItems = storeitems;
            WallProperties = wallProperties;
            UpdateTotalAmount();
        }

        private void UpdateTotalAmount()
        {
            double totalAmount = 0;
            foreach (BuildingMaterial mat in MaterialItems)
            {
                totalAmount += mat.Amount;
            }
            TxtBlockAmount.Text = totalAmount.ToString("F2");
        }


        private void PdfButton_Click(object sender, RoutedEventArgs e)
        {

            // Create and configure SaveFileDialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            saveFileDialog.Title = "Save PDF";
            saveFileDialog.FileName = "Autojenzi_Building Materials.pdf";

            if (saveFileDialog.ShowDialog() == true)
            {
                Export.ExportToPdf(this, saveFileDialog.FileName);
            }

        }

        private void ExcelButton_Click(object sender, RoutedEventArgs e)
        {

            // Sample data extraction for demonstration purposes
            var wallProperties = WallProperties.ToList();
            var materialItems = MaterialItems.ToList();

            Export.ExportToExcel(materialItems, wallProperties);

        }


        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MaterialItems.Clear();
            WallProperties.Clear();

            this.Close();
        }


    }
}
