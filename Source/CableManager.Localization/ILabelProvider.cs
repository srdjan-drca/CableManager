namespace CableManager.Localization
{
   public interface ILabelProvider
   {
      void SetCulture(string cultureCode);

      string GetCulture();

      string this[string key] { get; }
   }
}
