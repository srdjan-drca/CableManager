using System.Collections.Generic;
using Spire.Pdf;

namespace CableManager.PriceLoader.Helpers
{
   public static class PdfDocumentHelper
   {
      public static List<string> GetPagesWithPrices(string pdfPath)
      {
         var document = new PdfDocument();
         var allPagesContent = new List<string>();

         document.LoadFromFile(pdfPath);

         foreach (PdfPageBase documentPage in document.Pages)
         {
            string pageContent = documentPage.ExtractText();

            if (pageContent.Contains("mm2"))
            {
               allPagesContent.Add(pageContent);
            }
         }

         return allPagesContent;
      }
   }
}
