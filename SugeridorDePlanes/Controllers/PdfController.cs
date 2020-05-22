using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Spire.Pdf;
using IronPdf;
using jsreport.Types;

namespace Telefonica.SugeridorDePlanes.Controllers
{
    public class PdfController : Controller
    {
        public IActionResult Index()
        {

            test230();

            var randomName = Path.GetRandomFileName() + ".pdf";
            var pdfOutfiles = @"C:\Users\Usuario\Desktop\Proyecto\pdf\outPDFFiles\" + randomName;
            var file1 = @"C:\Users\Usuario\Desktop\Proyecto\pdf\destPdf\prueba.pdf";
            var file2 = @"C:\Users\Usuario\Desktop\Proyecto\pdf\PrimerPagina.pdf";
            var stringList = new string[2] { file1, file2 };

            var urlPdf = MergePdf();



            var stream = new FileStream(urlPdf, FileMode.Open, FileAccess.Read);
             return File(stream, "application/pdf");






           // return View();
        }



        public string MergePdf()
        {

            Spire.Pdf.PdfDocument document = new Spire.Pdf.PdfDocument();
            string[] lstFiles = new string[2];
            lstFiles[0] = @"C:\Users\Usuario\Desktop\Proyecto\pdf\destPdf\prueba.pdf";
            lstFiles[1] = @"C:\Users\Usuario\Desktop\Proyecto\pdf\PrimerPagina.pdf";


            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage;
            //  string outputPdfPath = @"C:\Users\Usuario\Desktop\Proyecto\pdf\outPDFFiles\" + Path.GetRandomFileName() + ".pdf";
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            string outputPdfPath = Path.Combine(tempDirectory, Path.GetRandomFileName() + ".pdf");

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
                string sourceFile = outputPdfPath;
                string destFile = System.IO.Path.Combine(tempDirectory, "Prueba.pdf");
              //  System.IO.Directory.CreateDirectory(targetPath);
                System.IO.File.Copy(sourceFile, destFile, true);

                ///
                return destFile;

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int get_pageCcount(string file)
        {
            using (StreamReader sr = new StreamReader(System.IO.File.OpenRead(file)))
            {
                Regex regex = new Regex(@"/Type\s*/Page[^s]");
                MatchCollection matches = regex.Matches(sr.ReadToEnd());

                return matches.Count;
            }
        }


        public static void test230()
        {
            // Render any HTML fragment or document to HTML
            var Renderer = new IronPdf.HtmlToPdf();
            var htmlUrl = @"C:\Users\Usuario\Desktop\Proyecto\pdf\html\PDFSugeridor\Pagina2.html";

            StreamReader objReader = new StreamReader(htmlUrl);
            string content = objReader.ReadToEnd();
            objReader.Close();

            var PDF = Renderer.RenderHtmlAsPdf(content);

            

          //  var PDF = IronPdf.HtmlToPdf.StaticRenderHTMLFileAsPdf(htmlUrl);

            var OutputPath = @"C:\Users\Usuario\Desktop\Proyecto\pdf\outPDFFiles\example.pdf";
            PDF.SaveAs(OutputPath);


        }

        protected async Task<IActionResult> RenderPDFAsync(string content)
        {
            var rs = new LocalReporting().UseBinary(JsReportBinary.GetBinary()).AsUtility().Create();

            var report = await rs.RenderAsync(new RenderRequest()
            {
                Template = new Template()
                {
                    Recipe = Recipe.ChromePdf,
                    Engine = Engine.None,
                    Content = content
                }
            });

            return new FileStreamResult(report.Content, new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/pdf"));
        }


    }
}