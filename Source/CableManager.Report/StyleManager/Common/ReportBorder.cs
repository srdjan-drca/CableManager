using System.Drawing;
using CableManager.Report.Helpers;

namespace CableManager.Report.StyleManager.Common
{
   public class ReportBorder : ReportStyleItem
   {
      public Color Color { get; set; }

      public float Width { get; set; }

      public BorderStyleEnum Style { get; set; }

      public BorderSideEnum Side { get; set; }

      public ReportBorder(float width, Color color, BorderSideEnum side = BorderSideEnum.None, BorderStyleEnum style = BorderStyleEnum.Solid)
      {
         Color = color;
         Width = width;
         Side = side;
         Style = style;
      }

      public override ReportStyleItem Clone()
      {
         return MemberwiseClone() as ReportBorder;
      }
   }
}