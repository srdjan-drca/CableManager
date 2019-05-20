using System.Collections.Generic;
using System.Drawing;
using CableManager.Report.Helpers;
using Spire.Pdf.Fields;
using Spire.Pdf.Graphics;

namespace CableManager.Report.Extensions
{
   public static class TypeConverterExtensions
   {
      private static readonly Dictionary<FontStyleEnum, FontStyle> FontMapping = new Dictionary<FontStyleEnum, FontStyle> {
         { FontStyleEnum.Regular, FontStyle.Regular },
         { FontStyleEnum.Bold, FontStyle.Bold },
         { FontStyleEnum.Italic, FontStyle.Italic },
         { FontStyleEnum.Underline, FontStyle.Underline },
         { FontStyleEnum.Striketrough, FontStyle.Strikeout }
      };

      private static readonly Dictionary<BorderStyleEnum, PdfBorderStyle> BorderMapping = new Dictionary<BorderStyleEnum, PdfBorderStyle> {
         { BorderStyleEnum.Solid, PdfBorderStyle.Solid },
         { BorderStyleEnum.Dashed, PdfBorderStyle.Dashed }
      };

      private static readonly Dictionary<HorizontalAlignment, PdfTextAlignment> AlignmentMapping = new Dictionary<HorizontalAlignment, PdfTextAlignment> {
         { HorizontalAlignment.Left, PdfTextAlignment.Left },
         { HorizontalAlignment.Right, PdfTextAlignment.Right },
         { HorizontalAlignment.Center, PdfTextAlignment.Center }
      };

      private static readonly Dictionary<VerticalAlignment, PdfVerticalAlignment> VerticalAlignmentMapping = new Dictionary<VerticalAlignment, PdfVerticalAlignment> {
         { VerticalAlignment.Bottom, PdfVerticalAlignment.Bottom },
         { VerticalAlignment.Middle, PdfVerticalAlignment.Middle },
         { VerticalAlignment.Top, PdfVerticalAlignment.Top }
      };

      public static FontStyle ToFontStyle(this FontStyleEnum fontStyleEnum)
      {
         FontStyle fontStyle;

         if (FontMapping.TryGetValue(fontStyleEnum, out fontStyle))
         {
            return fontStyle;
         }

         return (FontStyle)fontStyleEnum;
      }

      public static PdfBorderStyle ToBorderStyle(this BorderStyleEnum borderStyle)
      {
         return BorderMapping[borderStyle];
      }

      public static PdfTextAlignment ToPdfTextAlignment(this HorizontalAlignment hAlignment)
      {
         return AlignmentMapping[hAlignment];
      }

      public static PdfVerticalAlignment ToPdfVerticalAlignment(this VerticalAlignment vAlignment)
      {
         return VerticalAlignmentMapping[vAlignment];
      }

      public static PdfSolidBrush ToPdfBrush(this Color color)
      {
         return new PdfSolidBrush(color);
      }
   }
}