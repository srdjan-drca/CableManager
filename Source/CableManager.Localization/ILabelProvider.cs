namespace CableManager.Localization
{
   public interface ILabelProvider
   {
      void SetCulture(string cultureCode);

      string this[string key] { get; }
   }
}
