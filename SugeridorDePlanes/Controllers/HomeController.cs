using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telefonica.SugeridorDePlanes;
using Telefonica.SugeridorDePlanes.Code;
using AutoMapper;
using Telefonica.SugeridorDePlanes.Models.ApiModels;
using Telefonica.SugeridorDePlanes.Models.Usuarios;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Telefonica.SugeridorDePlanes.Controllers
{
    public class HomeController : Controller
    {
        private IManejoUsuario usuario;
        private readonly IMapper _mapper;
        private ITelefonicaService telefonicaApi;
        private List<SugeridorClientesModel> _clientList;
        private readonly IWebHostEnvironment _env;

        public HomeController(IMapper mapper, IManejoUsuario usuarioInterface, ITelefonicaService telefonicaService, IWebHostEnvironment env)
        {
            usuario = usuarioInterface;
            telefonicaApi = telefonicaService;
            _mapper = mapper;
            _clientList = new List<SugeridorClientesModel>();
            _env = env;
        }


        public async Task<IActionResult> Index()
        {
            return ReturnPdf();
            //string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            //Directory.CreateDirectory(tempDirectory);


            // codigo para copiar archivos
            /*  string fileName = "HTML.html";
              string tempName = Path.GetRandomFileName() + ".html";
              string sourcePath = @"C:\Users\Usuario\Desktop\Proyecto\pdf";
              string targetPath = @"C:\Users\Usuario\Desktop\Proyecto\pdf\temp";          


              string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
              string destFile = System.IO.Path.Combine(targetPath, tempName);
              System.IO.Directory.CreateDirectory(targetPath);
              System.IO.File.Copy(sourceFile, destFile, true);


              ////replace

              StreamReader objReader = new StreamReader(destFile);
              string content = objReader.ReadToEnd();
              objReader.Close();
              content = Regex.Replace(content, "{change}", "cambio");

              StreamWriter writer = new StreamWriter(destFile);
              writer.Write(content);
              writer.Close();

              ////*/



            return View("../Home/Detalles");



            // return GetPdf();


            //var clientList = await telefonicaApi.GetClientes();
            //List<SugeridorClientesModel> clientsModel = _mapper.Map<List<SugeridorClientes>, List<SugeridorClientesModel>>(clientList);
            //_clientList = clientsModel;
            //ViewData["clientList"] = clientsModel;

            //List<RecomendadorB2b> plansList = await telefonicaApi.GetSuggestedPlans();
            //var planMapped = _mapper.Map<List<RecomendadorB2b>, List<RecomendadorB2bModel>>(plansList);

            //return View("../Home/Index");
        }

        public IActionResult Get()
        {


            //codigo para copiar archivos
            var urlPdf = @"C:\Users\Usuario\Desktop\Proyecto\Plan de trabajo.pdf";
            var urlHtml = @"C:\Users\Usuario\Desktop\Proyecto\pdf\HTML.html";

            Document doc = new Document(PageSize.Letter);
            FileStream file = new FileStream("prueba.pdf", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            PdfWriter writer = PdfWriter.GetInstance(doc, file);
            doc.AddAuthor("autor");
            doc.AddTitle("titulo");
            doc.Open();
            doc.Add(new Phrase("esto es una prueba"));
            writer.Close();
            doc.Close();
            file.Dispose();
            var pdf = new FileStream("prueba.pdf", FileMode.Open, FileAccess.Read);
            return File(pdf, "application/pdf");

            var stream = new FileStream(urlHtml, FileMode.Open, FileAccess.Read);
            return File(stream, "application/pdf");

            return new FileStreamResult(stream, "application/pdf");
        }

        public IActionResult GetPdf()
        {
            var urlPdf = @"C:\Users\Usuario\Desktop\Proyecto\Plan de trabajo.pdf";
            var urlHtml = @"C:\Users\Usuario\Desktop\Proyecto\pdf\HTML.html";

            var textHtml = "<html><head></head><body>< p > Prueba Net.Core </ p ></ body ></ html > ";

            Document doc = new Document(PageSize.Letter);
            FileStream file = new FileStream("prueba.pdf", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            PdfWriter writer = PdfWriter.GetInstance(doc, file);
            doc.AddAuthor("autor");
            doc.AddTitle("titulo");
            doc.Open();
            doc.Add(new Phrase(textHtml));
            writer.Close();
            doc.Close();
            file.Dispose();
            var pdf = new FileStream("prueba.pdf", FileMode.Open, FileAccess.Read);
            return File(pdf, "application/pdf");

            // var stream = new FileStream(urlHtml, FileMode.Open, FileAccess.Read);
            // return File(stream, "application/pdf");

            //  return new FileStreamResult(stream, "application/pdf");
        }




        [HttpPost]
        public async Task<IActionResult> ShowPlans(string rut)
        {

            var clientList = await telefonicaApi.GetClientes();
            List<SugeridorClientesModel> clientsModel = _mapper.Map<List<SugeridorClientes>, List<SugeridorClientesModel>>(clientList);
            ViewData["clientList"] = clientsModel;

            List<RecomendadorB2b> plansList = await telefonicaApi.GetSuggestedPlansByRut(rut);
            var planMapped = _mapper.Map<List<RecomendadorB2b>, List<RecomendadorB2bModel>>(plansList);

            return View("../Home/Index", planMapped);
        }



        public IActionResult ReturnPdf()
        {
            HtmlToPdfConverter converter = new HtmlToPdfConverter();
            WebKitConverterSettings settings = new WebKitConverterSettings();
            settings.WebKitPath = Path.Combine(_env.ContentRootPath, "QtBinariesWindows");
            converter.ConverterSettings = settings;
            var urlHtml = @"C:\Users\Usuario\Desktop\Proyecto\pdf\HTML.html";



            Syncfusion.Pdf.PdfDocument document = converter.Convert(urlHtml);


            MemoryStream ms = new MemoryStream();
            document.Save(ms);
            document.Close(true);

            ms.Position = 0;


            FileStreamResult fileStreamResult = new FileStreamResult(ms, "application/pdf");
            fileStreamResult.FileDownloadName = "prueba.pdf";

            var testFile = fileStreamResult.FileStream;


            //// codigo para copiar el pdf a una carpeta

            var destPathPdf = @"C:\Users\Usuario\Desktop\Proyecto\pdf\destPdf";
            var destTempPdf = @"C:\Users\Usuario\Desktop\Proyecto\pdf\destPdf\prueba.pdf";

            //using (var stream = System.IO.File.Create(destPathPdf))
            //{
            //    testFile.CopyTo(stream);
            //}



            using (var fileStream = new FileStream(destTempPdf, FileMode.Create, FileAccess.Write))
            {
                fileStreamResult.FileStream.CopyTo(fileStream);
            }



            ///

            return fileStreamResult;
        }


        



    }
}
