using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SautinSoft.Document;
using Syncfusion.HtmlConverter;

namespace Telefonica.SugeridorDePlanes.Controllers
{
    public class Pdf2Controller : Controller
    {
        public IActionResult Index()
        {
           GetHtml();           
          
           return View();
        }

        public void GetHtml()
        {
            string fileName = "HTML.html";
            string tempName = Path.GetRandomFileName() + ".html";

            // path donde se encuentra el HTML original
            string sourcePath = @"C:\Users\Usuario\Desktop\Proyecto\pdf";
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string sourceFile = Path.Combine(sourcePath, fileName);
            string destFile = Path.Combine(tempDirectory, tempName);
            Directory.CreateDirectory(tempDirectory);
            System.IO.File.Copy(sourceFile, destFile, true);


            ////logica para hacer reemplazar

            StreamReader objReader = new StreamReader(destFile);
            string content = objReader.ReadToEnd();
            objReader.Close();
            content = Regex.Replace(content, "{change}", "prueba union");

            StreamWriter writer = new StreamWriter(destFile);
            writer.Write(content);
            writer.Close();

            ////
            ReturnPdf(tempDirectory, tempName);
        }       


        public void ReturnPdf(string htmlFolder, string htmlName)
        {
            var urlproyect = @"C:\Git\Sugeridor\SugeridorDePlanes";

            //var urlHtml = @"C:\Users\Usuario\Desktop\Proyecto\pdf\HTML.html";
            var urlHtml = Path.Combine(htmlFolder, htmlName);

            HtmlToPdfConverter converter = new HtmlToPdfConverter();
            WebKitConverterSettings settings = new WebKitConverterSettings();
            settings.WebKitPath = Path.Combine(urlproyect, "QtBinariesWindows");
            converter.ConverterSettings = settings;

            // se le pasa la url donde esta guardada temporalmente el html
            Syncfusion.Pdf.PdfDocument document = converter.Convert(urlHtml);
            MemoryStream ms = new MemoryStream();
            document.Save(ms);
            document.Close(true);
            ms.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(ms, "application/pdf");
            fileStreamResult.FileDownloadName = "prueba.pdf";

            var testFile = fileStreamResult.FileStream;


            //// codigo para copiar el pdf a una carpeta

          
            //var destTempPdf = @"C:\Users\Usuario\Desktop\Proyecto\pdf\destPdf\prueba.pdf";
            var destTempPdf = Path.Combine(htmlFolder, "firtsPage.pdf");

            using var fileStream = new FileStream(destTempPdf, FileMode.Create, FileAccess.Write);
            fileStreamResult.FileStream.CopyTo(fileStream);
            fileStream.Close();
            MergeMultipleDocuments(destTempPdf);
             //return fileStreamResult;
        }

        public static void MergeMultipleDocuments(string urlPdf)
        {
            string singlePDFPath = "Single.pdf";
            // string workingDir = @"C:\Users\Usuario\Desktop\Proyecto\pdf\outPDFFiles";

            List<string> supportedFiles = new List<string>();
            // var file1 = @"C:\Users\Usuario\Desktop\Proyecto\pdf\destPdf\prueba.pdf";
            var firstpartPdf = @"C:\Users\Usuario\Desktop\Proyecto\pdf\PrimerPagina.pdf";           

            supportedFiles.Add(firstpartPdf);
            supportedFiles.Add(urlPdf);

            DocumentCore singlePDF = new DocumentCore();
            foreach (string file in supportedFiles)
            {
                DocumentCore dc = DocumentCore.Load(file);
                ImportSession session = new ImportSession(dc, singlePDF, StyleImportingMode.KeepSourceFormatting);
                foreach (Section sourceSection in dc.Sections)
                {
                    Section importedSection = singlePDF.Import<Section>(sourceSection, true, session);

                    if (dc.Sections.IndexOf(sourceSection) == 0)
                        importedSection.PageSetup.SectionStart = SectionStart.NewPage;

                    singlePDF.Sections.Add(importedSection);
                }
            }
            singlePDF.Save(singlePDFPath);
            // Open the result for demonstration purposes.
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(singlePDFPath) { UseShellExecute = true });
        }


    }
}