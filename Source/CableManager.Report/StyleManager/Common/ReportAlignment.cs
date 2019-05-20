using CableManager.Report.Helpers;

namespace CableManager.Report.StyleManager.Common
{
   public class ReportAlignment : ReportStyleItem
   {
      public HorizontalAlignment Horizontal { get; set; }

      public VerticalAlignment Vertical { get; set; }

      public ReportAlignment(HorizontalAlignment horizontal = HorizontalAlignment.Left, VerticalAlignment vertical = VerticalAlignment.Middle)
      {
         Horizontal = horizontal;
         Vertical = vertical;
      }

      public override ReportStyleItem Clone()
      {
         return MemberwiseClone() as ReportAlignment;
      }
   }
}