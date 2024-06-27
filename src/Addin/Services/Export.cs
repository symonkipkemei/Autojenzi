using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Autojenzi.src.Addin.Services
{
    public static class Export
    {

        public static void ExportToExcel(List<BuildingMaterial> materialItems, List<WallProperties> wallProperties)
        {

            // Set the license context to non-commercial
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var wsWallProperties = package.Workbook.Worksheets.Add("Wall Properties");
                wsWallProperties.Cells.LoadFromCollection(wallProperties, true);

                var wsMaterialItems = package.Workbook.Worksheets.Add("Material Items");
                wsMaterialItems.Cells.LoadFromCollection(materialItems, true);

                var filePath = "MaterialData.xlsx";
                var file = new FileInfo(filePath);
                package.SaveAs(file);
                MessageBox.Show($"Data exported to {filePath}");


                // Automatically open the Excel file
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
        }


        public static void ExportToPdf(Window window,string pdfFilename)
        {
        
            // Specify a higher DPI for better resolution
            double dpi = 300;

            // Render the current window content to a bitmap with higher DPI
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)(window.ActualWidth * dpi / 96), (int)(window.ActualHeight * dpi / 96),
                dpi, dpi, PixelFormats.Pbgra32);

            renderBitmap.Render(window);

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
                pdf.Save(pdfFilename);

                MessageBox.Show($"PDF saved to {System.IO.Path.GetFullPath(pdfFilename)}", "PDF Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

    }
}
