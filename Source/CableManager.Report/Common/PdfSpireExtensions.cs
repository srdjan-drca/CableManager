using System;
using System.IO;
using System.Drawing;
using Spire.Pdf;
using Spire.Pdf.Grid;
using Spire.Pdf.Graphics;
using CableManager.Report.StyleManager;
using CableManager.Report.StyleManager.Cell;
using CableManager.Report.StyleManager.Common;

namespace CableManager.Report.Common
{
   public static class PdfSpireExtensions
   {
      public static void Add(this PdfPageBase page, PdfGrid pdfGrid, float x, float y)
      {
         pdfGrid.Draw(page, x, y);
      }

      public static void Add(this PdfPageBase page, Action<PdfPageBase> action)
      {
         action(page);
      }

      public static void AddText(this PdfPageBase page, string text, CellStyle style, float x, float y)
      {
         var font = new PdfTrueTypeFont(new Font(style.Font.Name, style.Font.Size, style.Font.Style.ToFontStyle()), true);
         var horizontalAlignment = style.Alignment.Horizontal.ToPdfTextAlignment();

         page.Canvas.DrawString(text, font, PdfBrushes.Black, x, y, new PdfStringFormat(horizontalAlignment));
      }

      public static void AddLine(this PdfPageBase page, float x1, float y1, float x2, float y2)
      {
         var pen = new PdfPen(PdfBrushes.Black, 0.75f);

         page.Canvas.DrawLine(pen, x1, y1, x2, y2);
      }

      public static PdfGridRow AddRow(this PdfGrid pdfGrid, float height = 0)
      {
         PdfGridRow gridRow = pdfGrid.Rows.Add();

         if (height != 0)
         {
            gridRow.Height = height;
         }

         return gridRow;
      }

      public static void AddCell(this PdfGridRow row, CellStyle cellStyle, int columnId, float value, int columnSpan = 1)
      {
         AddCell(row, cellStyle, columnId, value.ToString(), columnSpan);
      }

      public static void AddCell(this PdfGridRow row, CellStyle cellStyle, int columnId, int value, int columnSpan = 1)
      {
         AddCell(row, cellStyle, columnId, value.ToString(), columnSpan);
      }

      public static void AddCell(this PdfGridRow row, CellStyle cellStyle, int columnId, string value, int columnSpan = 1)
      {
         var font = new PdfTrueTypeFont(new Font(cellStyle.Font.Name, cellStyle.Font.Size, cellStyle.Font.Style.ToFontStyle()), true);
         var backgroundColor = new PdfSolidBrush(cellStyle.Background.Color);
         var pdfPen = new PdfPen(cellStyle.Border.Color, cellStyle.Border.Width);
         PdfTextAlignment horizontalAlignment = cellStyle.Alignment.Horizontal.ToPdfTextAlignment();
         PdfVerticalAlignment verticalAlignment = cellStyle.Alignment.Vertical.ToPdfVerticalAlignment();

         row.Cells[columnId].Value = value;
         row.Cells[columnId].ColumnSpan = columnSpan;
         row.Cells[columnId].Style.Font = font;
         row.Cells[columnId].Style.BackgroundBrush = backgroundColor;
         row.Cells[columnId].StringFormat = new PdfStringFormat(horizontalAlignment, verticalAlignment);

         SetBorders(row, cellStyle.Border, columnId, pdfPen);
      }

      #region ReportStyleItem

      public static CellStyle RemoveAllBorders(this ReportStyleItem style)
      {
         var cellStyle = style as CellStyle;

         if (cellStyle != null)
         {
            cellStyle.Border.Side = BorderSideEnum.None;
         }

         return cellStyle;
      }

      public static CellStyle SetLeftBorder(this ReportStyleItem style)
      {
         var cellStyle = style as CellStyle;

         if (cellStyle != null)
         {
            cellStyle.Border.Side |= BorderSideEnum.Left;
         }

         return cellStyle;
      }

      public static CellStyle SetTopBorder(this ReportStyleItem style)
      {
         var cellStyle = style as CellStyle;

         if (cellStyle != null)
         {
            cellStyle.Border.Side |= BorderSideEnum.Top;
         }

         return cellStyle;
      }

      public static CellStyle SetRightBorder(this ReportStyleItem style)
      {
         var cellStyle = style as CellStyle;

         if (cellStyle != null)
         {
            cellStyle.Border.Side |= BorderSideEnum.Right;
         }

         return cellStyle;
      }

      public static CellStyle SetBottomBorder(this ReportStyleItem style)
      {
         var cellStyle = style as CellStyle;

         if (cellStyle != null)
         {
            cellStyle.Border.Side |= BorderSideEnum.Bottom;
         }

         return cellStyle;
      }

      public static CellStyle SetBorderBrush(this ReportStyleItem style, Color color, float width)
      {
         var cellStyle = style as CellStyle;

         if (cellStyle != null)
         {
            cellStyle.Border.Color = color;
            cellStyle.Border.Width = width;
         }

         return cellStyle;
      }

      public static CellStyle SetCenterAlignment(this ReportStyleItem style)
      {
         var cellStyle = style as CellStyle;

         if (cellStyle != null)
         {
            cellStyle.Alignment.Horizontal = HorizontalAlignment.Center;
         }

         return cellStyle;
      }

      public static CellStyle SetRightAlignment(this ReportStyleItem style)
      {
         var cellStyle = style as CellStyle;

         if (cellStyle != null)
         {
            cellStyle.Alignment.Horizontal = HorizontalAlignment.Right;
         }

         return cellStyle;
      }

      public static CellStyle SetBold(this ReportStyleItem style)
      {
         var cellStyle = style as CellStyle;

         if (cellStyle != null)
         {
            cellStyle.Font.Style |= FontStyleEnum.Bold;
         }

         return cellStyle;
      }

      public static CellStyle SetUnderline(this ReportStyleItem style)
      {
         var cellStyle = style as CellStyle;

         if (cellStyle != null)
         {
            cellStyle.Font.Style |= FontStyleEnum.Underline;
         }

         return cellStyle;
      }

      public static CellStyle SetBackground(this ReportStyleItem style, CellBackground cellBackground)
      {
         var cellStyle = style as CellStyle;

         if (cellStyle != null)
         {
            cellStyle.Background = cellBackground;
         }

         return cellStyle;
      }

      #endregion ReportStyleItem

      public static void RemoveAllBorders(this PdfGridRow gridRow)
      {
         var border = new PdfPen(Color.Transparent);

         foreach (PdfGridCell gridRowCell in gridRow.Cells)
         {
            gridRowCell.Style.Borders.All = border;
         }
      }

      public static MemoryStream ToMemoryStream(this PdfDocument pdfDocument)
      {
         var memoryStream = new MemoryStream();
         pdfDocument.SaveToStream(memoryStream);
         memoryStream.Position = 0;
         pdfDocument.Close();

         return memoryStream;
      }

      private static void SetBorders(PdfGridRow row, ReportBorder border, int columnId, PdfPen pdfPen)
      {
         if (border.Side != BorderSideEnum.None)
         {
            if ((border.Side & BorderSideEnum.All) == BorderSideEnum.All)
            {
               row.Cells[columnId].Style.Borders.All = pdfPen;
            }
            else
            {
               row.Cells[columnId].Style.Borders.Left = new PdfPen(Color.Transparent, 0);
               row.Cells[columnId].Style.Borders.Top = new PdfPen(Color.Transparent, 0);
               row.Cells[columnId].Style.Borders.Right = new PdfPen(Color.Transparent, 0);
               row.Cells[columnId].Style.Borders.Bottom = new PdfPen(Color.Transparent, 0);

               if ((border.Side & BorderSideEnum.Left) == BorderSideEnum.Left)
               {
                  row.Cells[columnId].Style.Borders.Left = pdfPen;
               }

               if ((border.Side & BorderSideEnum.Top) == BorderSideEnum.Top)
               {
                  row.Cells[columnId].Style.Borders.Top = pdfPen;
               }

               if ((border.Side & BorderSideEnum.Right) == BorderSideEnum.Right)
               {
                  row.Cells[columnId].Style.Borders.Right = pdfPen;
               }

               if ((border.Side & BorderSideEnum.Bottom) == BorderSideEnum.Bottom)
               {
                  row.Cells[columnId].Style.Borders.Bottom = pdfPen;
               }
            }
         }
      }
   }
}