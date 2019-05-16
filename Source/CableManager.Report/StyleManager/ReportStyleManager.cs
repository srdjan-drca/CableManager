using CableManager.Report.Common;
using CableManager.Report.StyleManager.Cell;
using CableManager.Report.StyleManager.Common;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CableManager.Report.StyleManager
{
   public class ReportStyleManager
   {
      private static readonly Lazy<ReportStyleManager> Lazy = new Lazy<ReportStyleManager>(() => new ReportStyleManager());

      public static ReportStyleManager Instance => Lazy.Value;

      private ReportStyleManager()
      {
      }

      private Dictionary<CellStyleId, CellStyle> _cellStyles;

      public void Initialize()
      {
         var whiteSolidBackground = new CellBackground(Color.White);
         var graySolidBackground = new CellBackground(ColorHelper.GrayF5F5F5);
         var arial12Font = new ReportFont("Arial", 12, Color.Black);
         var arial10Font = new ReportFont("Arial", 10, Color.Black);
         var arial9Font = new ReportFont("Arial", 9, Color.Black);
         var arial8Font = new ReportFont("Arial", 8, Color.Black);
         var leftMiddleAlignment = new ReportAlignment();
         var transparentBorder = new ReportBorder(0, Color.Transparent, BorderSideEnum.All);

         _cellStyles = new Dictionary<CellStyleId, CellStyle> {
            { CellStyleId.Arial12BlackLeft, new CellStyle(whiteSolidBackground, arial12Font, leftMiddleAlignment, transparentBorder) },
            { CellStyleId.Arial10BlackLeft, new CellStyle(whiteSolidBackground, arial10Font, leftMiddleAlignment, transparentBorder) },
            { CellStyleId.Arial9BlackLeft, new CellStyle(whiteSolidBackground, arial9Font, leftMiddleAlignment, transparentBorder) },
            { CellStyleId.Arial8BlackLeft, new CellStyle(whiteSolidBackground, arial8Font, leftMiddleAlignment, transparentBorder) },
            { CellStyleId.Arial8BlackLeftGray, new CellStyle(graySolidBackground, arial8Font, leftMiddleAlignment, transparentBorder) }
         };
      }

      public CellStyle Get(CellStyleId cellStyleId)
      {
         CellStyle cellStyle;

         if (_cellStyles.TryGetValue(cellStyleId, out cellStyle))
         {
            return cellStyle;
         }

         throw new Exception("Cell style not found!");
      }
   }
}