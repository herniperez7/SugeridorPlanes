using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Syncfusion.HtmlConverter;
using Microsoft.AspNetCore.Mvc;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Spire.Pdf;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Linq;
using System.Globalization;

namespace Telefonica.SugeridorDePlanes.BusinessLogic
{
   public  class PdfLogic: IPdfLogic
    {
        private readonly IWebHostEnvironment _env;

        public PdfLogic(IWebHostEnvironment env)
        {
            _env = env;
        }

        public byte[] GeneratePdfFromHtml(List<MovilDevice> movilDevices)
        {            
            //directorio temporal que va a alojar provisoriamente los html que se van a modificar y los pdfs 
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);

            CopyFiles(tempDirectory); //copio los htmls desde el directorio base a un directorio temporal
            GenerateHtml(tempDirectory, movilDevices); //modifico las copias generadas
            ConvertHtmlToPdf(tempDirectory); //convierto las copias a pdf
            var bytesArrayPdf = MergePdf(tempDirectory); //mergeo los pdf generados con las primeras paginas estaticas del pdf completo y retorno un array de bytes de ese pdf completo
            return bytesArrayPdf;
        }


        /// <summary>
        /// Metodo para copiarl los html con sus imagenes para modificar
        /// </summary>
        /// <param name="directoryPath"></param>
        private void CopyFiles(string directoryPath)
        {
            var mainUrl = Path.Combine(_env.ContentRootPath, "wwwroot", "html");
            string[] filePaths = Directory.GetFiles(mainUrl);

            foreach (string file in filePaths)
            {
                FileInfo info = new FileInfo(file);
                if (File.Exists(info.FullName))
                {
                    var destPath = Path.Combine(directoryPath, info.Name);
                    File.Copy(info.FullName, destPath);
                }
            }
        }

        /// <summary>
        /// Metodo que genera los html con los datos de moviles y las tarifas
        /// </summary>
        private void GenerateHtml(string directoryUrl, List<MovilDevice> movilDevices)
        {    
                          
            var firstHtmlsourcePath = Path.Combine(directoryUrl, "Pagina1.html");
            var secondHtmlsourcePath = Path.Combine(directoryUrl, "Pagina2.html");
            string content = string.Empty;

            StreamReader objReader = new StreamReader(firstHtmlsourcePath);
            content = objReader.ReadToEnd();
            objReader.Close();
            var contetnTr = string.Empty;

            foreach (var movil in movilDevices)
            {
                contetnTr += "<tr><td>"+movil.Codigo+"</td><td>"+movil.Marca+"</td><td>"+movil.Stock+"</td></tr>";
            }

            content = Regex.Replace(content, "{devices}", contetnTr);

            StreamWriter writer = new StreamWriter(firstHtmlsourcePath);
            writer.Write(content);
            writer.Close();


            objReader = new StreamReader(secondHtmlsourcePath);
            content = objReader.ReadToEnd();
            objReader.Close();

            var today = DateTime.Today;
           // DateTime dt = DateTime.ParseExact(today.ToString(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            string formatDate = today.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
            content = Regex.Replace(content, "{date}", formatDate)
                           .Replace("{company}", "Empresa")
                           .Replace("{devicesCost}", "200")
                           .Replace("{monthlyFee}", "1000");

            writer = new StreamWriter(secondHtmlsourcePath);
            writer.Write(content);
            writer.Close();

        }        

        private void ConvertHtmlToPdf(string directoryPath)
        {
            var mainUrl = Path.Combine(_env.ContentRootPath, "wwwroot");
            HtmlToPdfConverter converter = new HtmlToPdfConverter();
            WebKitConverterSettings settings = new WebKitConverterSettings();
            settings.WebKitPath = Path.Combine(mainUrl, "QtBinariesWindows");
            converter.ConverterSettings = settings;
            string[] filePaths = Directory.GetFiles(directoryPath);

            foreach (string file in filePaths)
            {
                FileInfo info = new FileInfo(file);
                if (File.Exists(info.FullName))
                {
                    if (info.Extension.Equals(".html"))
                    {
                        Syncfusion.Pdf.PdfDocument document = converter.Convert(info.FullName);
                        MemoryStream ms = new MemoryStream();                   
                        document.Save(ms);
                        document.Close(true);
                         ms.Position = 0;
                        FileStreamResult fileStreamResult = new FileStreamResult(ms, "application/pdf");
                        // fileStreamResult.FileDownloadName = info.Name;

                        var fileName = info.Name.Split(".")[0] + ".pdf";
                        var destSource = Path.Combine(directoryPath, fileName);
                        var fileStream = new FileStream(destSource, FileMode.Create, FileAccess.Write);
                        fileStreamResult.FileStream.CopyTo(fileStream);
                        fileStream.Close();
                    }
                }
            }
        }

        private byte[] MergePdf(string directoryPath)
        {
            Spire.Pdf.PdfDocument document = new Spire.Pdf.PdfDocument();
            var mainUrl = Path.Combine(_env.ContentRootPath, "wwwroot");
            var mainPdf = Path.Combine(mainUrl, "pdf", "PropuestaComercial.pdf");
            string[] lstFiles = new string[3];
            lstFiles[0] = mainPdf;
            lstFiles[1] = Path.Combine(directoryPath, "pagina1.pdf");
            lstFiles[2] = Path.Combine(directoryPath, "pagina2.pdf");
            
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage;            
            string outputPdfPath = Path.Combine(directoryPath, "Presupuesto.pdf");
            sourceDocument = new Document();
            FileStream fs = new FileStream(outputPdfPath, System.IO.FileMode.OpenOrCreate);            
            pdfCopyProvider = new PdfCopy(sourceDocument, fs);
            
            //Open the output file
            sourceDocument.Open();

            try
            {      
                foreach (var item in lstFiles)
                {                   
                    document.LoadFromFile(item);
                    int pages = document.Pages.Count;

                    reader = new PdfReader(item);
                    //Add pages of current file
                    for (int i = 1; i <= pages; i++)
                    {
                        importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                        pdfCopyProvider.AddPage(importedPage);

                    }
                    reader.Close();
                }

                sourceDocument.Close();
                pdfCopyProvider.Close();
                document.Dispose();               
                fs.Close();                
                string pdfFilePath = outputPdfPath;
                byte[] bytes = File.ReadAllBytes(pdfFilePath);

                //elimino el directorio temporal
                Directory.Delete(directoryPath, true);

                return bytes;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
