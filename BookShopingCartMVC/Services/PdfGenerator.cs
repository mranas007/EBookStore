using BookShopingCartMVC.Services.IServices;
using SelectPdf;

namespace BookShopingCartMVC.Services
{
    public class PdfGenerator : IPdfGenerator
    {
        /// <summary>  
        /// Convert the HTML content to PDF format using Select.Pdf.  
        /// This converter is used for convert the Order list into PDF format.  
        /// </summary>  
        public byte[] GeneratePdfForOrderList(string htmlContent)
        {
            try
            {
                // Create a new PDF converter with minimal settings
                HtmlToPdf converter = new HtmlToPdf();

                // Basic converter options
                converter.Options.PdfPageSize = PdfPageSize.A4;
                converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
                converter.Options.MarginTop = 20;
                converter.Options.MarginBottom = 20;
                converter.Options.MarginLeft = 20;
                converter.Options.MarginRight = 20;

                // Convert HTML to PDF  
                PdfDocument doc = converter.ConvertHtmlString(htmlContent);

                // Save to memory stream  
                using (MemoryStream ms = new MemoryStream())
                {
                    doc.Save(ms);
                    doc.Close();
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"PDF generation failed: {ex.Message}", ex);
            }
        }
    }
}
