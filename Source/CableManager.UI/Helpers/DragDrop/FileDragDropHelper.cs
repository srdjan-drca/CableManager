﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace CableManager.UI.Helpers.DragDrop
{
   public class FileDragDropHelper
   {
      public static readonly DependencyProperty IsFileDragDropEnabledProperty =
         DependencyProperty.RegisterAttached("IsFileDragDropEnabled", typeof(bool), typeof(FileDragDropHelper), new PropertyMetadata(OnFileDragDropEnabled));

      public static readonly DependencyProperty FileDragDropTargetProperty =
         DependencyProperty.RegisterAttached("FileDragDropTarget", typeof(object), typeof(FileDragDropHelper), null);

      public static bool GetIsFileDragDropEnabled(DependencyObject obj)
      {
         return (bool)obj.GetValue(IsFileDragDropEnabledProperty);
      }

      public static void SetIsFileDragDropEnabled(DependencyObject obj, bool value)
      {
         obj.SetValue(IsFileDragDropEnabledProperty, value);
      }

      public static bool GetFileDragDropTarget(DependencyObject obj)
      {
         return (bool)obj.GetValue(FileDragDropTargetProperty);
      }

      public static void SetFileDragDropTarget(DependencyObject obj, bool value)
      {
         obj.SetValue(FileDragDropTargetProperty, value);
      }

      private static void OnFileDragDropEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
         if (e.NewValue == e.OldValue) return;
         var control = d as Control;
         if (control != null) control.Drop += OnDrop;
      }

      private static void OnDrop(object sender, DragEventArgs dragEventArgs)
      {
         var dependencyObject = sender as DependencyObject;
         if (dependencyObject == null)
         {
            return;
         }

         object target = dependencyObject.GetValue(FileDragDropTargetProperty);
         IFileDragDropTarget fileTarget = target as IFileDragDropTarget;

         if (fileTarget != null)
         {
            if (dragEventArgs.Data.GetDataPresent(DataFormats.FileDrop))
            {
               fileTarget.OnFileDrop((string[])dragEventArgs.Data.GetData(DataFormats.FileDrop));
            }
         }
         else
         {
            throw new Exception("FileDragDropTarget object must be of type IFileDragDropTarget");
         }
      }
   }
}