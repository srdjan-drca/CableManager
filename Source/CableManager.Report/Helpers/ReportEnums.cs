using System;

namespace CableManager.Report.Helpers
{
   public enum CellStyleId
   {
      Arial12BlackLeft = 0,
      Arial10BlackLeft,
      Arial9BlackLeft,
      Arial8BlackLeft,
      Arial8BlackLeftGray
   }

   public enum HorizontalAlignment
   {
      Left = 0,
      Center,
      Right,
      General
   }

   public enum VerticalAlignment
   {
      Top = 0,
      Middle,
      Bottom
   }

   public enum FillStyleEnum
   {
      Solid = 0,
      Gradient
   }

   public enum BorderStyleEnum
   {
      Solid = 0,
      Dashed
   }

   [Flags]
   public enum BorderSideEnum
   {
      None = 0,
      Bottom = 1,
      Left = 2,
      Top = 4,
      Right = 8,
      All = 15
   }

   [Flags]
   public enum FontStyleEnum
   {
      Regular = 0,
      Bold = 1,
      Italic = 2,
      Underline = 4,
      Striketrough = 8
   }
}