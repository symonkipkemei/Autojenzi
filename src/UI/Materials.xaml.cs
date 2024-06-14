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

namespace Autojenzi.src.UI
{
    /// <summary>
    /// Interaction logic for Materials.xaml
    /// </summary>
    public partial class Materials : Window
    {
        public ObservableCollection<BuildingMaterial> MaterialItems {  get; set; }

        public ObservableCollection<WallProperties> WallProperties { get; set; }

        public Materials(ObservableCollection<BuildingMaterial> storeitems, ObservableCollection<WallProperties> wallProperties)
        {
            InitializeComponent();
            if (storeitems == null)
            {
                MessageBox.Show("Store items cannot be null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DataContext = this;
            MaterialItems = storeitems;
            WallProperties = wallProperties;

        }


        private void PdfButton_Click(object sender, RoutedEventArgs e)
        {
            // Specify a higher DPI for better resolution
            double dpi = 300;

            // Render the current window content to a bitmap with higher DPI
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)(this.ActualWidth * dpi / 96), (int)(this.ActualHeight * dpi / 96),
                dpi, dpi, PixelFormats.Pbgra32);

            renderBitmap.Render(this);

            // Encode the rendered bitmap to a PNG stream
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Save(stream);
                stream.Position = 0;

                // Create a new PDF document
                PdfDocument pdf = new PdfDocument();
                PdfPage pdfPage = pdf.AddPage();
                pdfPage.Width = XUnit.FromPoint(renderBitmap.PixelWidth * 72 / dpi);
                pdfPage.Height = XUnit.FromPoint(renderBitmap.PixelHeight * 72 / dpi);
                XGraphics graph = XGraphics.FromPdfPage(pdfPage);

                // Load the rendered bitmap into an XImage
                XImage xImage = XImage.FromStream(stream);
                graph.DrawImage(xImage, 0, 0, pdfPage.Width, pdfPage.Height);

                // Save the PDF document
                string pdfFilename = "Materials.pdf";
                pdf.Save(pdfFilename);

                MessageBox.Show($"PDF saved to {System.IO.Path.GetFullPath(pdfFilename)}", "PDF Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}
