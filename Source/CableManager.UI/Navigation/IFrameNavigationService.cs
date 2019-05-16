using GalaSoft.MvvmLight.Views;

namespace CableManager.UI.Navigation
{
   public interface IFrameNavigationService : INavigationService
   {
      object Parameter { get; }
   }
}