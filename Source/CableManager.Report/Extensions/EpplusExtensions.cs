using System;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;

namespace CableManager.Report.Extensions
{
   public static class EpplusExtensions
   {
      public static void InsertImage(this ExcelWorksheet worksheet,
         MemoryStream imageAsMemoryStream, string name, int pixelTop, int pixelLeft, int pixelWidth, int pixelHeight)
      {
         Image image = Image.FromStream(imageAsMemoryStream);
         ExcelPicture picture = worksheet.Drawings.AddPicture(name, image);

         picture.SetPosition(pixelTop, pixelLeft);
         picture.SetSize(pixelWidth, pixelHeight);
      }

      public static void SetColumnsWidth(this ExcelWorksheet worksheet, int fromCol, int toCol, double width)
      {
         for (int columnCounter = fromCol; columnCounter <= toCol; columnCounter++)
         {
            worksheet.Column(columnCounter).Width = width;
         }
      }

      public static void SetValue(this ExcelRange excelRange, object value, bool showValueIfZero = false)
      {
         if (showValueIfZero || value != null)
         {
            Type valueType = value.GetType();

            if (valueType == typeof(string))
            {
               SetValue(excelRange, (string)value);
            }
            else if (valueType == typeof(DateTime))
            {
               SetValue(excelRange, (DateTime?)value);
            }
            else if (valueType == typeof(double))
            {
               SetValue(excelRange, (double)value, showValueIfZero);
            }
            else if (valueType == typeof(float))
            {
               SetValue(excelRange, (float)value, showValueIfZero);
            }
            else if (valueType == typeof(decimal))
            {
               SetValue(excelRange, (decimal)value, showValueIfZero);
            }
            else if (valueType == typeof(int))
            {
               SetValue(excelRange, (int)value, showValueIfZero);
            }
            else
            {
               excelRange.Value = value.ToString();
            }
         }
      }

      public static void SetValue(this ExcelRange excelRange, int value, bool showValueIfZero = false, string valueIfZero = null)
      {
         if (showValueIfZero || Math.Abs(value) > 0)
         {
            excelRange.Value = value;
         }
         else
         {
            if (valueIfZero != null)
            {
               excelRange.Value = valueIfZero;
            }
         }
      }

      public static void SetValue(this ExcelRange excelRange, int value, string numberFormat, string valueIfZero = null)
      {
         if (value > 0)
         {
            excelRange.Value = value;
            excelRange.Style.Numberformat.Format = numberFormat;
         }
         else
         {
            if (valueIfZero != null)
            {
               excelRange.Value = valueIfZero;
            }
         }
      }

      public static void SetValue(this ExcelRange excelRange, double value, bool showValueIfZero = false, string valueIfZero = null)
      {
         if (showValueIfZero || Math.Abs(value) > 0)
         {
            excelRange.Value = value;
         }
         else
         {
            if (valueIfZero != null)
            {
               excelRange.Value = valueIfZero;
            }
         }
      }

      public static void SetValue(this ExcelRange excelRange, double value, string numberFormat, string valueIfZero = null)
      {
         if (Math.Abs(value) > 0)
         {
            excelRange.Value = value;
            excelRange.Style.Numberformat.Format = numberFormat;
         }
         else
         {
            if (valueIfZero != null)
            {
               excelRange.Value = valueIfZero;
            }
         }
      }

      public static void SetValue(this ExcelRange excelRange, float value, bool showValueIfZero = false, string valueIfZero = null)
      {
         if (showValueIfZero || value > 0)
         {
            excelRange.Value = value;
         }
         else
         {
            if (valueIfZero != null)
            {
               excelRange.Value = valueIfZero;
            }
         }
      }

      public static void SetValue(this ExcelRange excelRange, float value, string numberFormat, string valueIfZero = null)
      {
         if (value > 0)
         {
            excelRange.Value = value;
            excelRange.Style.Numberformat.Format = numberFormat;
         }
         else
         {
            if (valueIfZero != null)
            {
               excelRange.Value = valueIfZero;
            }
         }
      }

      public static void SetValue(this ExcelRange excelRange, decimal value, bool showValueIfZero = false, string valueIfZero = null)
      {
         if (showValueIfZero || value > 0)
         {
            excelRange.Value = value;
         }
         else
         {
            if (valueIfZero != null)
            {
               excelRange.Value = valueIfZero;
            }
         }
      }

      public static void SetValue(this ExcelRange excelRange, decimal value, string numberFormat, string valueIfZero = null)
      {
         if (value > 0)
         {
            excelRange.Value = value;
            excelRange.Style.Numberformat.Format = numberFormat;
         }
         else
         {
            if (valueIfZero != null)
            {
               excelRange.Value = valueIfZero;
            }
         }
      }

      public static void SetValue(this ExcelRange excelRange, string value)
      {
         if (value != null)
         {
            excelRange.Value = value;
         }
      }

      public static void SetValue(this ExcelRange excelRange, string value, string numberFormat)
      {
         if (value != null)
         {
            excelRange.Value = value;
            excelRange.Style.Numberformat.Format = numberFormat;
         }
      }

      public static void SetValue(this ExcelRange excelRange, DateTime? value)
      {
         if (value != null)
         {
            excelRange.Value = value;
         }
      }

      public static void SetValue(this ExcelRange excelRange, DateTime? value, string numberFormat)
      {
         if (value != null)
         {
            excelRange.Value = value;
            excelRange.Style.Numberformat.Format = numberFormat;
         }
      }
   }
}
