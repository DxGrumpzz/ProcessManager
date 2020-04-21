﻿namespace ProcessManager.UI
{
    using System.Windows;

    /// <summary>
    /// An attached property that displays a placeholder text when a textbox is "empty" of user text
    /// </summary>
    public static class PlaceHolder
    {
        public static string GetText(DependencyObject obj)
        {
            return (string)obj.GetValue(TextProperty);
        }

        public static void SetText(DependencyObject obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached(
                "Text",
                typeof(string),
                typeof(PlaceHolder),
                new PropertyMetadata(string.Empty));
    };
};