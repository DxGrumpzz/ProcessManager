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
                // Apply a uniform margin for every control in panel
                foreach (Control control in panel.Children)
                {
                    control.Margin = (Thickness)e.NewValue;
                };

                // Unhook the event after finishing
                panel.Initialized -= eventHandler;
            };

            // Hook  the event
            panel.Initialized += eventHandler;

        }
    };
};
