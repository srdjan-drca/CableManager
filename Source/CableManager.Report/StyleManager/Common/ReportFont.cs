using CableManager.Report.Common;
using System.Drawing;

namespace CableManager.Report.StyleManager.Common
{
   public class ReportFont : ReportStyleItem
   {
      public string Name { get; set; }

      public float Size { get; set; }

      public Color Color { get; set; }

      public FontStyleEnum Style { get; set; }

      public ReportFont(string name, float size, Color color, FontStyleEnum style = FontStyleEnum.Regular)
      {
         Name = name;
         Size = size;
         Color = color;
         Style = style;
      }

      public override ReportStyleItem Clone()
      {
         return MemberwiseClone() as ReportFont;
      }
   }
}