namespace ProcessManager.UI
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Input;


    /// <summary>
    /// An attached property that will execute an <see cref="ICommand"/> when a control fires a <see cref="UIElement.MouseEnter"/> event
    /// </summary>
    public static class MouseAttachedProperty
    {
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(MyPropertyProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(MyPropertyProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(MouseAttachedProperty),
                new PropertyMetadata(default(ICommand), CommandChangedCallback));


        private static void CommandChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Don't do anything if the control doesn't derive from UIElement
            if (!(d is UIElement uiElement))
                return;

            // If the value isn't an ICommand derivative 
            if (!(e.NewValue is ICommand))
                Debugger.Break();

            // Bind MouseLeave event and execute the command
            uiElement.MouseEnter += (object sender, MouseEventArgs mouseEvent) =>
            {
                ((ICommand)e.NewValue).Execute(null);
            };
        }
    };

    /// <summary>
    /// An attached property that will execute an <see cref="ICommand"/> when a control fires a <see cref="UIElement.MouseLeave"/> event
    /// </summary>
    public static class MouseLeaveAttachedProperty
    {
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(MyPropertyProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(MyPropertyProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(MouseLeaveAttachedProperty),
                new PropertyMetadata(default(ICommand), CommandChangedCallback));


        private static void CommandChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Don't do anything if the control doesn't derive from UIElement
            if (!(d is UIElement uiElement))
                return;

            // If the value isn't an ICommand derivative 
            if (!(e.NewValue is ICommand))
            {
                Debugger.Break();
                return;
            }

            // Bind MouseLeave event and execute the command
            uiElement.MouseLeave += (object sender, MouseEventArgs mouseEvent) =>
            {
                ((ICommand)e.NewValue).Execute(null);
            };
        }
    };

};
