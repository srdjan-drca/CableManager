using System.Drawing;
using CableManager.Report.Helpers;

namespace CableManager.Report.StyleManager.Cell
{
   public class CellBackground : ReportStyleItem
   {
      public Color Color { get; set; }

      public FillStyleEnum Pattern { get; set; }

      public CellBackground(Color color, FillStyleEnum pattern = FillStyleEnum.Solid)
      {
         Color = color;
         Pattern = pattern;
      }

      public override ReportStyleItem Clone()
      {
         return MemberwiseClone() as CellBackground;
      }
   }
}