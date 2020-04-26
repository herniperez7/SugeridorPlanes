using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Telefonica.SugeridorDePlanes.BusinessLogic
{
    public interface IPdfLogic : IDisposable
    {
        void MergePdf(Stream outputPdfStream, IEnumerable<string> pdfFilePaths);
    }
}
