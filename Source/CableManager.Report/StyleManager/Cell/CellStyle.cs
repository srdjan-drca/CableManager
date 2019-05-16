using CableManager.Report.StyleManager.Common;

namespace CableManager.Report.StyleManager.Cell
{
   public class CellStyle : ReportStyleItem
   {
      public CellBackground Background { get; set; }

      public ReportFont Font { get; set; }

      public ReportAlignment Alignment { get; set; }

      public ReportBorder Border { get; set; }

      public CellStyle(CellBackground background, ReportFont font, ReportAlignment alignment, ReportBorder border)
      {
         Background = background.Clone() as CellBackground;
         Font = font.Clone() as ReportFont;
         Alignment = alignment.Clone() as ReportAlignment;
         Border = border.Clone() as ReportBorder;
      }

      public override ReportStyleItem Clone()
      {
         var background = Background.Clone() as CellBackground;
         var font = Font.Clone() as ReportFont;
         var alignment = Alignment.Clone() as ReportAlignment;
         var border = Border.Clone() as ReportBorder;

         return new CellStyle(background, font, alignment, border);
      }
   }
}