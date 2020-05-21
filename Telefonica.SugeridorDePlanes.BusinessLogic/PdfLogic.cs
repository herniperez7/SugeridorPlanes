using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Telefonica.SugeridorDePlanes.BusinessLogic
{
   public  class PdfLogic: IPdfLogic
    {

        public void GeneratePdfFromHtml()
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


        }
        




    }
}
