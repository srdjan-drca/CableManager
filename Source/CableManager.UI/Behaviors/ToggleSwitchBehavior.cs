using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Input;

namespace CableManager.UI.Behaviors
{
   public class ToggleSwitchBehavior
   {
      public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
         "Command", typeof(ICommand), typeof(ToggleSwitchBehavior), new UIPropertyMetadata(null, OnIsExternalChanged));

      public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
         "CommandParameter", typeof(object), typeof(ToggleSwitchBehavior), new UIPropertyMetadata(null, OnIsExternalChanged));

      public static ICommand GetCommand(DependencyObject obj)
      {
         return (ICommand)obj.GetValue(CommandProperty);
      }

      public static void SetCommand(DependencyObject obj, ICommand value)
      {
         obj.SetValue(CommandProperty, value);
      }

      public static object GetCommandParameter(DependencyObject obj)
      {
         return obj.GetValue(CommandParameterProperty);
      }

      public static void SetCommandParameter(DependencyObject obj, object value)
      {
         obj.SetValue(CommandParameterProperty, value);
      }

      private static void OnIsExternalChanged(object sender, DependencyPropertyChangedEventArgs args)
      {
         var toggleSwitch = sender as ToggleSwitch;

         if (toggleSwitch == null) return;

         if (args.NewValue != null)
            toggleSwitch.IsCheckedChanged += IsCheckedChangedHandler;
         else
            toggleSwitch.IsCheckedChanged -= IsCheckedChangedHandler;
      }

      private static void IsCheckedChangedHandler(object sender, EventArgs e)
      {
         var command = GetCommand((ToggleSwitch)sender);
         var commandParameter = GetCommandParameter((ToggleSwitch)sender);

         if (command.CanExecute(null))
         {
            command.Execute(commandParameter);
         }
      }
   }
}