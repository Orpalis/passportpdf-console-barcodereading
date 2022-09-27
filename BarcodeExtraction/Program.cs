using PassportPDF.Api;
using PassportPDF.Client;


namespace BarcodeExtraction
{

    public class BarcodeExtractor
    {
        static void displayBarcodesInfo(PassportPDF.Model.ReadBarcodesResponse barcodes)
        {
            foreach (var barcode in barcodes.Barcodes)
            {
                Console.WriteLine("Barcode type : {0}", barcode.Type);
                Console.WriteLine("Barcode symbology : {0}", barcode.Barcode1DSymbology);
                Console.WriteLine("X_left : {0}", barcode.X1);
                Console.WriteLine("Y_top : {0}", barcode.Y1);
                Console.WriteLine("X_right : {0}", barcode.X3);
                Console.WriteLine("Y_down : {0}", barcode.Y3);
                Console.WriteLine("Barcode data : {0}", barcode.Data);
                Console.WriteLine("-------------------");
            }
        }

        static void Main(string[] args)
        {
            GlobalConfiguration.ApiKey = "YOUR-PASSPORT-CODE";
            DocumentApi api = new();

            var uri = "https://passportpdfapi.com/test/invoice_with_barcode.pdf";
            var document = api.DocumentLoadFromURIAsync(new PassportPDF.Model.LoadDocumentFromURIParameters(uri)).Result;

            var imageSupportedFormats = new ImageApi().ImageGetSupportedFileExtensionsAsync().Result;

            if (imageSupportedFormats.Value.Contains(document.DocumentFormat.ToString()))
            {
                ImageApi imageApi = new();

                var barcodes = imageApi.ImageReadBarcodesAsync(new PassportPDF.Model.ImageReadBarcodesParameters(document.FileId, "*")).Result;

                displayBarcodesInfo(barcodes);
            }
            else if (document.DocumentFormat == PassportPDF.Model.DocumentFormat.PDF)
            {
                PDFApi pdfApi = new();

                var barcodes = pdfApi.ReadBarcodesAsync(new PassportPDF.Model.PdfReadBarcodesParameters(document.FileId, "*")).Result;

                displayBarcodesInfo(barcodes);
            }
            else
            {
                var pdfImportSupportedFormats = new PDFApi().GetPDFImportSupportedFileExtensionsAsync().Result;

                if (pdfImportSupportedFormats.Value.Contains(document.DocumentFormat.ToString()))
                {
                    PDFApi pdfApi = new PDFApi();

                    var convertedDocument = pdfApi.LoadDocumentAsPDFFromHTTPAsync(new PassportPDF.Model.PdfLoadDocumentFromHTTPParameters(uri)).Result;

                    if(convertedDocument != null)
                    {
                        var barcodes = pdfApi.ReadBarcodesAsync(new PassportPDF.Model.PdfReadBarcodesParameters(convertedDocument.FileId, "*")).Result;

                        displayBarcodesInfo(barcodes);
                    }
                    
                }
                else
                {
                    Console.WriteLine("Document format not supported!");
                }
                

            }


        }
    }
}


