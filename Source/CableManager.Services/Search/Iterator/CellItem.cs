namespace CableManager.Services.Search.Iterator
{
   public class CellItem
   {
      public int Worksheet { get; set; }

      public int Row { get; set; }

      public int Column { get; set; }

      public string Text { get; set; }

      public CellItem(int worksheet, int row, int column, string text)
      {
         Worksheet = worksheet;
         Row = row;
         Column = column;
         Text = text;
      }
   }
}
