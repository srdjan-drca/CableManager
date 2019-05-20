using System.IO;
using Spire.Pdf;
using Spire.Pdf.Grid;
using CableManager.Localization;
using CableManager.Report.Extensions;
using CableManager.Report.Helpers;
using CableManager.Report.Models;
using CableManager.Report.StyleManager;
using CableManager.Report.StyleManager.Cell;

namespace CableManager.Report.Generators.Pdf.Sections
{
   public abstract class BasePdfSection
   {
      private CellStyle _arial8BlackLeft;

      protected BaseReportModel BaseReportModel;

      protected ILabelProvider LabelProvider;

      protected BasePdfSection(BaseReportModel baseReportModel)
      {
         BaseReportModel = baseReportModel;
         LabelProvider = baseReportModel.LabelProvider;

         CreateCellStyles();
      }

      public abstract MemoryStream GenerateContent();

      protected static PdfGrid CreateGrid(params float[] columnsWidth)
      {
         var table = new PdfGrid();
         int columnCounter = 0;

         table.Columns.Add(columnsWidth.Length);

         foreach (PdfGridColumn column in table.Columns)
         {
            column.Width = columnsWidth[columnCounter++];
         }

         return table;
      }

      protected void ReportFooter(PdfPageBase page)
      {
         float pageWidth = page.ActualSize.Width;
         float pageHeight = page.ActualSize.Height;
         string pageNumber = $" {1} - {1}";

         page.AddText(pageNumber, _arial8BlackLeft, (pageWidth / 2) - 20, pageHeight - 50);
      }

      private void CreateCellStyles()
      {
         _arial8BlackLeft = ReportStyleManager.Instance.Get(CellStyleId.Arial8BlackLeft);
      }
   }
}