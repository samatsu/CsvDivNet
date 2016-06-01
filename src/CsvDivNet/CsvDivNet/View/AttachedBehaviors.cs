using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Input;
using System.Windows.Data;

namespace CsvDivNet.View
{
    class MarginSetter
    {
        public static readonly DependencyProperty MarginProperty = DependencyProperty.RegisterAttached(
            "Margin", typeof(Thickness), typeof(MarginSetter), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange, OnPropertyChanged));

        public static void SetMargin(DependencyObject element, Thickness value)
        {
            element.SetValue(MarginProperty, value);
        }
        public static Thickness GetMargin(DependencyObject element)
        {
            return (Thickness)element.GetValue(MarginProperty);
        }

        public static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Panel p = d as Panel;

            if (p != null && p.IsInitialized == false)
            {
                p.Initialized += new EventHandler(panel_Initialized);
            }
        }

        static void panel_Initialized(object sender, EventArgs e)
        {
            Panel p = sender as Panel;
            foreach (UIElement item in p.Children)
            {
                if (item is FrameworkElement)
                {
                    (item as FrameworkElement).Margin = GetMargin(p);
                }
            }

        }
    }

    enum DropKind
    {
        File,
        Directory
    }
    class Drop
    {
        public static readonly DependencyProperty KindProperty = DependencyProperty.RegisterAttached(
            "Kind", typeof(DropKind?), typeof(Drop), new PropertyMetadata(OnPropertyChanged));

        public static void SetKind(UIElement element, DropKind value)
        {
            element.SetValue(KindProperty, value);
        }
        public static DropKind GetKind(UIElement element)
        {
            return (DropKind)element.GetValue(KindProperty);
        }

        public static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox text = d as TextBox;
            if (text == null) return;

            if (e.OldValue != null)
            {
                text.AllowDrop = false;
                text.RemoveHandler(UIElement.PreviewDragOverEvent, new DragEventHandler(OnPreviewDragEnter));
                text.RemoveHandler(UIElement.PreviewDragOverEvent, new DragEventHandler(OnPreviewDragOver));
                text.RemoveHandler(UIElement.DropEvent, new DragEventHandler(OnDrop));
            }
            if (e.NewValue != null)
            {
                text.AllowDrop = true;
                text.AddHandler(UIElement.PreviewDragOverEvent, new DragEventHandler(OnPreviewDragEnter));
                text.AddHandler(UIElement.PreviewDragOverEvent, new DragEventHandler(OnPreviewDragOver));
                text.AddHandler(UIElement.DropEvent, new DragEventHandler(OnDrop));
            }
        }

        private static void OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Handled = IsAllowedDropKind(sender, e);
        }

        private static void OnPreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = IsAllowedDropKind(sender, e);
        }

        private static bool IsAllowedDropKind(object sender, DragEventArgs e)
        {
            bool isAllowedDropKind = false;
            TextBox text = sender as TextBox;
            if (text == null) return isAllowedDropKind;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files != null && files.Length > 0)
                {
                    string file = files[0];
                    DropKind kind = GetKind(text);
                    if (kind == DropKind.Directory && Directory.Exists(file))
                    {
                        isAllowedDropKind = true;
                    }
                    else if (kind == DropKind.File && File.Exists(file))
                    {
                        isAllowedDropKind = true;
                    }
                }
            }
            return isAllowedDropKind;
        }
        private static void OnDrop(object sender, DragEventArgs e)
        {
            TextBox text = sender as TextBox;
            if (text == null) return;

            e.Handled = e.Data.GetDataPresent(DataFormats.FileDrop);
            if (e.Handled)
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files != null && files.Length > 0)
                {
                    string file = files[0];
                    DropKind kind = GetKind(text);
                    if (kind == DropKind.Directory && Directory.Exists(file))
                    {
                        text.SetValue(TextBox.TextProperty, file);
                        BindingExpression exp = text.GetBindingExpression(TextBox.TextProperty);
                        if (exp != null)
                        {
                            exp.UpdateSource();
                            ICommand droppedCommand = GetDroppedCommand(text);
                            if (droppedCommand != null && droppedCommand.CanExecute(null))
                            {
                                droppedCommand.Execute(null);
                            }
                        }
                    }
                    else if (kind == DropKind.File && File.Exists(file))
                    {
                        text.SetValue(TextBox.TextProperty, file);
                        // UpdateSourceTrigger が既定値の場合 TextはLostFocus
                        // が発生するまで ソースプロパティが変更されない。
                        // UpdateSourceTrigger を PropertyChangted にするか、
                        // 下例のように強制的に更新
                        BindingExpression exp = text.GetBindingExpression(TextBox.TextProperty);
                        if (exp != null)
                        {
                            exp.UpdateSource();
                            ICommand droppedCommand = GetDroppedCommand(text);
                            if (droppedCommand != null && droppedCommand.CanExecute(null))
                            {
                                droppedCommand.Execute(null);
                            }
                        }
                    }
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public static readonly DependencyProperty DroppedCommandProperty = DependencyProperty.RegisterAttached(
            "DroppedCommand", typeof(ICommand), typeof(Drop),
             new PropertyMetadata(null));
        public static void SetDroppedCommand(UIElement element, ICommand value)
        {
            element.SetValue(DroppedCommandProperty, value);
        }
        public static ICommand GetDroppedCommand(UIElement element)
        {
            return element.GetValue(DroppedCommandProperty) as ICommand;
        }

    }
}
