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

namespace Telefonica.SugeridorDePlanes.BusinessLogic
{
   public  class PdfLogic: IPdfLogic
    {
        private readonly IWebHostEnvironment _env;

        public PdfLogic(IWebHostEnvironment env)
        {
            _env = env;
        }

        public FileStream GeneratePdfFromHtml(List<MovilDevice> movilDevices)
        {
            var mainUrl = Path.Combine(_env.ContentRootPath, "wwwroot");      
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);

            CopyFiles(tempDirectory);
            GenerateHtml(tempDirectory, movilDevices);
            convertHtmlToPdf(tempDirectory);
            MergePdf(tempDirectory);

            var pdfSource = Path.Combine(tempDirectory, "Presupuesto.pdf");
            var stream = new FileStream(pdfSource, FileMode.Open, FileAccess.Read);

            return stream;

        }
        
        /// <summary>
        /// Metodo que genera los html con los datos de moviles y las tarifas
        /// </summary>
        private void GenerateHtml(string directoryUrl, List<MovilDevice> movilDevices)
        {        
                          
            var firstHtmlsourcePath = Path.Combine(directoryUrl, "Pagina1.html");
            var secondHtmlsourcePath = Path.Combine(directoryUrl, "Pagina2.html");

            StreamReader objReader = new StreamReader(firstHtmlsourcePath);
            string content = objReader.ReadToEnd();
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
        }


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

        private void convertHtmlToPdf(string directoryPath)
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

        private void MergePdf(string directoryPath)
        {
            Spire.Pdf.PdfDocument document = new Spire.Pdf.PdfDocument();
            var mainUrl = Path.Combine(_env.ContentRootPath, "wwwroot");
            var mainPdf = Path.Combine(mainUrl, "pdf", "PropuestaComercial.pdf");
            string[] lstFiles = new string[3];
            lstFiles[0] = Path.Combine(directoryPath, "pagina1.pdf");
            lstFiles[1] = Path.Combine(directoryPath, "pagina1.pdf");
            lstFiles[2] = mainPdf;
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage;            
            string outputPdfPath = Path.Combine(directoryPath, "Presupuesto.pdf");
            sourceDocument = new Document();
            pdfCopyProvider = new PdfCopy(sourceDocument, new System.IO.FileStream(outputPdfPath, System.IO.FileMode.OpenOrCreate));

            //Open the output file
            sourceDocument.Open();

            try
            {
                //Loop through the files list

                foreach (var item in lstFiles)
                {
                    //int pages = get_pageCcount(item);
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


                //At the end save the output file

                sourceDocument.Close();
                reader.Close();
                pdfCopyProvider.Close();
                document.Dispose();

                sourceDocument = new Document();



                ///  lo muevo de directorio

                //  string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                // Directory.CreateDirectory(tempDirectory);
              //  string sourceFile = outputPdfPath;
               // string destFile = System.IO.Path.Combine(directoryPath, "Prueba.pdf");
                //  System.IO.Directory.CreateDirectory(targetPath);
               // System.IO.File.Copy(sourceFile, destFile, true);

                ///
              //  return destFile;

            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
