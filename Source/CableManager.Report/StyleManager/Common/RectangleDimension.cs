namespace CableManager.Report.StyleManager.Common
{
   public class RectangleDimension : ReportStyleItem
   {
      public float X { get; set; }

      public float Y { get; set; }

      public float Width { get; set; }

      public float Height { get; set; }

      public RectangleDimension(float x, float y, float width, float height)
      {
         X = x;
         Y = y;
         Width = width;
         Height = height;
      }

      public override ReportStyleItem Clone()
      {
         return MemberwiseClone() as RectangleDimension;
      }
   }
}