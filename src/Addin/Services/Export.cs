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
                try
                {
                    //custom blue color for excel header
                    var customBlueColor = System.Drawing.Color.FromArgb(1, 123, 204);


                    //Wall properties
                    var wsWallProperties = package.Workbook.Worksheets.Add("Element Quantities");
                    wsWallProperties.Cells.LoadFromCollection(wallProperties, true);
                    wsWallProperties.Cells[wsWallProperties.Dimension.Address].AutoFitColumns();

                    // Make the first row bold
                    using (var range = wsWallProperties.Cells[1, 1, 1, wsWallProperties.Dimension.End.Column])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(customBlueColor);
                        range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    }

                    //materials
                    var wsMaterialItems = package.Workbook.Worksheets.Add("Material Quantities");
                    var materialItemExports = materialItems.Select(mi => new MaterialItemExport
                    {
                        Name = mi.Name,
                        UnitOfProduct = mi.UnitOfProduct,
                        ProductQuantity = mi.ProductQuantity,
                        Rate = mi.Rate,
                        Amount = mi.Amount
                    }).ToList();

                    wsMaterialItems.Cells.LoadFromCollection(materialItemExports, true);
                    wsMaterialItems.Cells[wsMaterialItems.Dimension.Address].AutoFitColumns();

                    // Make the first row bold
                    using (var range = wsMaterialItems.Cells[1, 1, 1, wsMaterialItems.Dimension.End.Column])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(customBlueColor);
                        range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    }

                    //Get the desktoppath that is consistent for all folders
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    var filePath = Path.Combine(desktopPath, "Autojenzi_MaterialData.xlsx");
                    var file = new FileInfo(filePath);

           
                    if (file.Exists)
                        {
                            file.Delete(); // Overwrite if the file already exists
                        }


                    package.SaveAs(file);
                    MessageBox.Show($"Data exported to {filePath}");

                    // Automatically open the Excel file
                    Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });

                }

                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"Access to the path is denied: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }


                catch (IOException ex)
                {
                    MessageBox.Show($"I/O error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        public static void ExportToPdf(Window window,string pdfFilename)
        {
        
            // Specify a higher DPI for better resolution
            double dpi = 300;

            // Render the current window content to a bitmap with higher DPI
            double renderHeight = window.ActualHeight - 80;
            double renderWidth = window.ActualWidth - 15;
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)(renderWidth * dpi / 96), (int)(renderHeight * dpi / 96),
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
