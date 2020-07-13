using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.HtmlConverter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;
using Telefonica.SugeridorDePlanes.Dto.Dto;
using Telefonica.SugeridorDePlanes.Resources.helpers;

namespace Telefonica.SugeridorDePlanes.BusinessLogic.Services
{
    public class PdfLogic : IPdfLogic
    {
        private readonly IWebHostEnvironment _env;
        private ILogLogic _logLogic;
        private int mobilesPages;

        public PdfLogic(IWebHostEnvironment env, ILogLogic logLogic)
        {
            _env = env;
            _logLogic = logLogic;
           
        }

        public byte[] GeneratePdfFromHtml(List<DevicePymes> movilDevices, List<OfertPlan> planList, string companyName, double devicePayment)
        {
            //directorio temporal que va a alojar provisoriamente los html que se van a modificar y los pdfs 
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(tempDirectory);
                CopyFiles(tempDirectory, movilDevices); //copio los htmls desde el directorio base a un directorio temporal
                GenerateHtml(tempDirectory, movilDevices, planList, companyName, devicePayment); //modifico las copias generadas
                ConvertHtmlToPdf(tempDirectory);  // ConvertHtmlToPdf(tempDirectory); //convierto las copias a pdf
                var bytesArrayPdf = MergePdf(tempDirectory); //mergeo los pdf generados con las primeras paginas estaticas del pdf completo y retorno un array de bytes de ese pdf completo
                return bytesArrayPdf;
            }
            catch (Exception ex)
            {
                var extraData = new { directory = tempDirectory, step = "first" };                
               _logLogic.InsertLog(new Log("generar pdf", ex.Message, extraData));
                DeleteDirectory(tempDirectory);
                throw ex;
            }

        }


        /// <summary>
        /// Metodo para copiarl los html con sus imagenes para modificar
        /// </summary>
        /// <param name="directoryPath"></param>
        private void CopyFiles(string directoryPath, List<DevicePymes> movilDevices)
        {
            var movilPdfList = GroupedMovilList(movilDevices);
            mobilesPages = CalculatePages(movilPdfList.Count);            
            var destPath = string.Empty;
            try
            {
                var mainUrl = Path.Combine(_env.ContentRootPath, "wwwroot", "html");
                string[] filePaths = Directory.GetFiles(mainUrl);

                foreach (string file in filePaths)
                {
                    FileInfo info = new FileInfo(file);
                    if (File.Exists(info.FullName))
                    {
                        if (info.Name.Equals("mobiles.html"))
                        {
                            for (int i = 0; i < mobilesPages; i++)
                            {
                                var fileName = info.Name.Split(".")[0] + i.ToString() + ".html";
                                destPath = Path.Combine(directoryPath, fileName);
                                File.Copy(info.FullName, destPath);
                            }
                        }
                        else {
                            destPath = Path.Combine(directoryPath, info.Name);
                            File.Copy(info.FullName, destPath);
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                DeleteDirectory(directoryPath);
                throw ex;
            }
        }

        /// <summary>
        /// Metodo que genera los html con los datos de moviles y las tarifas
        /// </summary>
        private void GenerateHtml(string directoryUrl, List<DevicePymes> mobileDevices, List<OfertPlan> planList, string companyName, double devicePayment)
        {


            try
            {
                GenerateHtmlMobiles(directoryUrl, mobileDevices);

                var firstHtmlsourcePath = Path.Combine(directoryUrl, "Subsidy.html");              
                string content = string.Empty;  
                decimal devicesCost = GetDevicesCost(mobileDevices);
                var contentPlans = GetContentPlans(planList);
                var monthyFee = GetMothlyFee(planList);
                double subsidio = (double)devicesCost;
                StreamReader objReader = new StreamReader(firstHtmlsourcePath);
                content = objReader.ReadToEnd();
                objReader.Close();

                var today = DateTime.Today;
                string formatDate = today.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
                content = Regex.Replace(content, "{date}", formatDate)
                               .Replace("{company}", companyName)
                               .Replace("{plans}", contentPlans)                              
                               .Replace("{subsidio}", TelefonicaHelper.FormatCultureDouble(subsidio))
                               .Replace("{monthlyFee}", TelefonicaHelper.FormatCultureDouble((double)monthyFee))
                               .Replace("{devicePayment}", TelefonicaHelper.FormatCultureDouble(devicePayment));

                StreamWriter writer = new StreamWriter(firstHtmlsourcePath);
                writer.Write(content);
                writer.Close();
            }
            catch (Exception ex)
            {
                DeleteDirectory(directoryUrl);
                throw ex;
            }
        }


        private void GenerateHtmlMobiles(string directoryUrl, List<DevicePymes> mobileDevices)
        {
            try
            {
                var movilPdfList = GroupedMovilList(mobileDevices);
                int mobilesCount = movilPdfList.Count;                
                var mobileHtml = string.Empty;
                var contentMoviles = string.Empty;
                string content = string.Empty;

                for (int i = 0; i < mobilesPages; i++)
                {
                    int j = i + 1;
                    mobileHtml = Path.Combine(directoryUrl, "mobiles" + i.ToString() + ".html");


                    StreamReader objReader = new StreamReader(mobileHtml);
                    content = objReader.ReadToEnd();
                    objReader.Close();                   

                    int from = i * 9 + i; 
                    int to = j * 9 + i;  

                    if (to > mobilesCount)
                    {
                        to = mobilesCount;
                    }

                    contentMoviles = GetContentMoviles(mobileDevices, from, to);

                    content = Regex.Replace(content, "{devices}", contentMoviles);

                    StreamWriter writer = new StreamWriter(mobileHtml);
                    writer.Write(content);
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }



        private decimal GetMothlyFee(List<OfertPlan> planList)
        {
            decimal monthlyFee = 0;

            foreach (var plan in planList)
            {
                monthlyFee += plan.TMM_s_iva;
            }

            return monthlyFee;
        }

        private decimal GetDevicesCost(List<DevicePymes> movilDevices)
        {
            decimal devicesCost = 0;

            foreach (var movil in movilDevices)
            {
                devicesCost += movil.PrecioSinIva;
            }

            return devicesCost;
        }

        private string GetContentMoviles(List<DevicePymes> mobileDevices, int from, int to)
        {
            var movilPdfList = GroupedMovilList(mobileDevices);
            string content = string.Empty;

            if (mobileDevices.Count() == 0)
            {
                content += "<tr><td colspan='3'>No se han agregado equipos</td><td>";
            }

            for (int i = from; i < to; i++)
            {
                content += "<tr><td>" + movilPdfList[i].Marca + "</td><td>" + movilPdfList[i].Modelo + "</td><td>" + movilPdfList[i].Cantidad + "</td></tr>";

            }
            return content;
        }

        private string GetContentPlans(List<OfertPlan> planList)
        {
            var planPdfList = GroupedPlanList(planList);
            var contentPlans = string.Empty;
            var countPLans = planPdfList.Count;
            var isPair = false;
            if (planPdfList.Count % 2 == 00) isPair = true;

            if (isPair)
            {
                for (var i = 0; i < countPLans - 1; i = i + 2)
                {
                    contentPlans += "<tr>";
                    contentPlans += "<td><p>" + planPdfList[i].Cantidad + " lineas <b>" + planPdfList[i].Bono + "GB <b/></p><p>Plan celular</p></td>";
                    contentPlans += "<td><p>" + planPdfList[i + 1].Cantidad + " lineas <b>" + planPdfList[i + 1].Bono + "GB <b/></p><p>Plan celular</p></td>";
                    contentPlans += "</tr>";

                }
            }
            else
            {
                if (countPLans > 1)
                {
                    for (var i = 0; i < countPLans - 2; i = i + 2)
                    {
                        contentPlans += "<tr>";
                        contentPlans += "<td><p>" + planPdfList[i].Cantidad + " lineas <b>" + planPdfList[i].Bono + "GB </b></p><p>Plan celular</p></td>";
                        contentPlans += "<td><p>" + planPdfList[i + 1].Cantidad + " lineas <b>" + planPdfList[i + 1].Bono + "GB <b/></p><p>Plan celular</p></td>";
                        contentPlans += "</tr>";

                    }
                    contentPlans += "<tr>";
                    contentPlans += "<td colspan=\"2\" class=\"centerTd\"><div><p>" + planPdfList[countPLans - 1].Cantidad + " lineas <b>" + planPdfList[countPLans - 1].Bono + "GB <b/></p><p>Plan celular</p></div></td>";
                    contentPlans += "</tr>";
                }
                else
                {
                    contentPlans += "<tr>";
                    contentPlans += "<td colspan=\"2\" class=\"centerTd\"><div><p>" + planPdfList[countPLans - 1].Cantidad + " lineas <b>" + planPdfList[countPLans - 1].Bono + "GB <b/></p><p>Plan celular</p></div></td>";
                    contentPlans += "</tr>";
                }


            }

            return contentPlans;
        }
        private List<MovilPdf> GroupedMovilList(List<DevicePymes> mobileDevices)
        {
            try
            {
                var movilPdfList = new List<MovilPdf>();

                movilPdfList = mobileDevices
                .GroupBy(m => m.CodigoEquipo)
                .Select(m => new MovilPdf
                {
                    Codigo = m.First().CodigoEquipo,
                    Marca = m.First().Marca,
                    Modelo = m.First().Nombre,
                    Cantidad = m.Count()
                }
                ).ToList();

                return movilPdfList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<PlanPdf> GroupedPlanList(List<OfertPlan> planList)
        {
            var planPdfList = new List<PlanPdf>();

            planPdfList = planList
            .GroupBy(m => m.Plan)
            .Select(m => new PlanPdf
            {
                Plan = m.First().Plan,
                Bono = m.First().Bono,
                Cantidad = m.Count()
            }
            ).ToList();

            return planPdfList;
        }

        private void ConvertHtmlToPdf(string directoryPath)
        {
            try
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


                            var fileName = info.Name.Split(".")[0] + ".pdf";
                            var destSource = Path.Combine(directoryPath, fileName);
                            var fileStream = new FileStream(destSource, FileMode.Create, FileAccess.Write);
                            fileStreamResult.FileStream.CopyTo(fileStream);
                            fileStream.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DeleteDirectory(directoryPath);
                throw ex;
            }
        }

        private byte[] MergePdf(string directoryPath)
        {
            try
            {
                Spire.Pdf.PdfDocument document = new Spire.Pdf.PdfDocument();
                var mainUrl = Path.Combine(_env.ContentRootPath, "wwwroot");
                var mainPdf = Path.Combine(mainUrl, "pdf", "PropuestaComercial.pdf");
                int totalFiles = mobilesPages + 2;
                string[] lstFiles = new string[totalFiles];
                lstFiles[0] = mainPdf;
                lstFiles[1] = Path.Combine(directoryPath, "Subsidy.pdf");

                int index = 0;
                for (int i = 2; i < totalFiles; i++)
                {
                    lstFiles[i] = Path.Combine(directoryPath, "mobiles" + index.ToString() + ".pdf");
                    index++;
                }            

                PdfReader reader = null;
                Document sourceDocument = null;
                PdfCopy pdfCopyProvider = null;
                PdfImportedPage importedPage;
                string outputPdfPath = Path.Combine(directoryPath, "Presupuesto.pdf");
                sourceDocument = new Document();
                FileStream fs = new FileStream(outputPdfPath, FileMode.OpenOrCreate);
                pdfCopyProvider = new PdfCopy(sourceDocument, fs);

                //Open the output file
                sourceDocument.Open();

                foreach (var item in lstFiles)
                {
                    document.LoadFromFile(item);
                    int pages = document.Pages.Count;

                    reader = new PdfReader(item);
                    FileInfo info = new FileInfo(item);
                    var fileName = info.Name.Split(".")[0];
                    fileName = fileName.Substring(0, 6); // me quedo solo con la palabra "mobile";

                    if (fileName.Equals("mobile"))
                    {
                        importedPage = pdfCopyProvider.GetImportedPage(reader, 1);
                        pdfCopyProvider.AddPage(importedPage);
                    }
                    else 
                    {
                        //Add pages of current file
                        for (int i = 1; i <= pages; i++)
                        {
                            importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                            pdfCopyProvider.AddPage(importedPage);
                        }
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
                DeleteDirectory(directoryPath);

                return bytes;
            }

            catch (Exception ex)
            {
                DeleteDirectory(directoryPath);
                throw ex;
            }
        }

        private void DeleteDirectory(string directoryPath)
        {
            bool exist = Directory.Exists(directoryPath);
            if (exist)
            {
                //elimino el directorio temporal
                Directory.Delete(directoryPath, true);
            }
        }

        private int CalculatePages(int count)
        {
            int pages = count;
            int cant = 1;
            while (pages > 10)
            {
                pages -= 10;
                cant++;
            }

            return cant;
        }
    }
}
