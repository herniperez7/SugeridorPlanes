using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SautinSoft.Document;
using Syncfusion.HtmlConverter;

namespace Telefonica.SugeridorDePlanes.Controllers
{
    public class PdfController : Controller
    {
        private readonly IWebHostEnvironment _env;
        public PdfController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            return ReturnPdf();
        }




        public IActionResult ReturnPdf()
        {

            // var urlHtml = Path.Combine(_env.ContentRootPath, @"wwwroot\html\Pagina1.html");
            var urlHtml = Path.Combine(_env.ContentRootPath, @"wwwroot\html\Pagina2.html");

            HtmlToPdfConverter converter = new HtmlToPdfConverter();
            WebKitConverterSettings settings = new WebKitConverterSettings();
            settings.WebKitPath = Path.Combine(_env.ContentRootPath, "QtBinariesWindows");
            converter.ConverterSettings = settings;

            // se le pasa la url donde esta guardada temporalmente el html
            Syncfusion.Pdf.PdfDocument document = converter.Convert(urlHtml);
            MemoryStream ms = new MemoryStream();
            document.Save(ms);
            document.Close(true);
            ms.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(ms, "application/pdf");
            fileStreamResult.FileDownloadName = Path.GetRandomFileName() + ".pdf";


            return fileStreamResult;

            //// codigo para copiar el pdf a una carpeta


            /* var destTempPdf = @"C:\Users\Usuario\Desktop\Proyecto\pdf\outPDFFiles";
             destTempPdf = Path.Combine(destTempPdf, Path.GetRandomFileName() + ".html");

             using var fileStream = new FileStream(destTempPdf, FileMode.Create, FileAccess.Write);

            // return File(fileStream, "application/pdf");
             fileStreamResult.FileStream.CopyTo(fileStream);
             fileStream.Close();

             return fileStream;*/

        }




    }
}