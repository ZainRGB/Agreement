using DinkToPdf;
using DinkToPdf.Contracts;

namespace Agreement.Services
{
    public class PdfGenerationService
    {
        private readonly IConverter _converter;

        public PdfGenerationService(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] GeneratePdf(string htmlContent, string title = "Document")
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10, Bottom = 10 },
                DocumentTitle = title
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = {
                    FontSize = 9,
                    Right = "Page [page] of [toPage]",
                    Line = true
                },
                FooterSettings = {
                    FontSize = 9,
                    Center = "Generated on " + DateTime.Now.ToString("yyyy-MM-dd")
                }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            return _converter.Convert(pdf);
        }
    }
}