namespace ProcessManager.UI
{
    using System;
    using System.Windows;
    using System.Windows.Controls;


    /// <summary>
    /// Applies a uniform margin to every control inside of a panel
    /// </summary>
    public static class UniformMargin
    {
        public static Thickness Margin { get; set; }

        public static Thickness GetMargin(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(MarginProperty);
        }

        public static void SetMargin(DependencyObject obj, Thickness value)
        {
            obj.SetValue(MarginProperty, value);
        }

        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.RegisterAttached(
                nameof(Margin),
                typeof(Thickness),
                typeof(UniformMargin),
                new PropertyMetadata(default(Thickness), MarginChangedCallback));

        private static void MarginChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Don't do anything if the control isn't a panel
            if (!(d is Panel panel))
                return;

            // Unironically using a local function instead of an event variable
            void eventHandler(object sender, EventArgs evnt)
            {
                // Store the margin that will be added to the controls
                var newMargin = (Thickness)e.NewValue;

                // Apply a uniform margin for every control in panel
                foreach (FrameworkElement frameworkElement in panel.Children)
                {
                    // Get the control's current margin
                    var currentContorlMargin = frameworkElement.Margin;
                    
                    // Add the new margin to the current margin
                    currentContorlMargin.Top += newMargin.Top;
                    currentContorlMargin.Left += newMargin.Left;
                    currentContorlMargin.Bottom += newMargin.Bottom;
                    currentContorlMargin.Right += newMargin.Right;

                    // Apply the new margin values
                    frameworkElement.Margin = currentContorlMargin;
                };

                // Unhook the event after finishing
                panel.Initialized -= eventHandler;
            };

            // Hook  the event
            panel.Initialized += eventHandler;

        }
    };
};
