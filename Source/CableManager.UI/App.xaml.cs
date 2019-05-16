using System.Windows;
using CableManager.UI.Configuration;

namespace CableManager.UI
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {
      protected override void OnStartup(StartupEventArgs e)
      {
         BootStrapper.Initialize();
      }
   }
}