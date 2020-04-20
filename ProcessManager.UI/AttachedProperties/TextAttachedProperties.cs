namespace ProcessManager.UI
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// An attached property that displays a placeholder text when a textbox is "empty" of user text
    /// </summary>
    public static class PlaceHolder
    {

        private static bool GetPlaceHolderFlag(DependencyObject obj)
        {
            return (bool)obj.GetValue(_placeHolderFlagProperty);
        }

        private static void SetPlaceHolderFlag(DependencyObject obj, bool value)
        {
            obj.SetValue(_placeHolderFlagProperty, value);
        }

        /// <summary>
        /// A "simple" boolean flag that is used to indicate if a textbox has a placeholder showing
        /// </summary>
        private static readonly DependencyProperty _placeHolderFlagProperty =
            DependencyProperty.RegisterAttached(
                "PlaceHolderFlag",
                typeof(bool),
                typeof(PlaceHolder),
                new PropertyMetadata(default(bool)));


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
                new PropertyMetadata(string.Empty, PlaceHolderPropertyChanged));


        private static void PlaceHolderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Don't continue if control isn't a textbox
            if (!(d is TextBox textBox))
            {
                Debugger.Break();
                return;
            };

            return;

            // Wait for control to *load not initialize.
            // Difference is initialize event is called much earlier than Loaded,
            // So what happens is the Placeholder value doesn't show until user interacts with the control
            textBox.Loaded += (object sender, RoutedEventArgs evnt) =>
            {
                SetPlaceHolderValue(textBox, (string)e.NewValue);
            };

            // When focus is returned to the textbox
            textBox.GotFocus += (object sender, RoutedEventArgs evnt) =>
            {
                // Get placeholder flag
                bool value = GetPlaceHolderFlag(textBox);

                // If the placeholder is currently visible
                if (value == true)
                {
                    // Clone the textbox's foreground colour
                    var newForeground = textBox.Foreground.Clone();

                    // Set the text's opacity back to normal
                    newForeground.Opacity = 1D;
                    newForeground.Freeze();

                    // Set the new foreground colour
                    textBox.Foreground = newForeground;

                    // Reset textbox text to an empty string
                    textBox.Text = string.Empty;

                    // Set placeholder flag to false
                    SetPlaceHolderFlag(textBox, false);
                };
            };

            // When focus is lost from textbox
            textBox.LostFocus += (object sender, RoutedEventArgs evnt) =>
            {
                // Try to set the placeholder value
                SetPlaceHolderValue(textBox, (string)e.NewValue);
            };
        }

        /// <summary>
        /// Sets a text placeholder on a textbox
        /// </summary>
        /// <param name="textBox"> The textbox to apply the placeholder onto </param>
        /// <param name="placeholder"> The Placeholder text to apply </param>
        private static void SetPlaceHolderValue(TextBox textBox, string placeholder)
        {
            // If the textbox isn't empty
            if (!(string.IsNullOrWhiteSpace(textBox.Text)))
                return;

            // Clone foreground and set opacity to be a little invisible
            var newForgeround = textBox.Foreground.Clone();
            newForgeround.Opacity = .5;
            newForgeround.Freeze();

            // Set the new foreground
            textBox.Foreground = newForgeround;

            // Set the placeholde
            textBox.Text = placeholder;

            // Set placeholder flag to true
            SetPlaceHolderFlag(textBox, true);

        }

    };

};