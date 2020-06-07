namespace ProcessManager.UI
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Input;


    /// <summary>
    /// An attached property that will execute an <see cref="ICommand"/> when a control fires a <see cref="UIElement.MouseEnter"/> event
    /// </summary>
    public static class MouseEnterAttachedProperty
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
                typeof(MouseEnterAttachedProperty),
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


    /// <summary>
    /// A class that contains attached properties for a mouse moved event
    /// </summary>
    public class MouseMoved
    {

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }


        /// <summary>
        /// An attached property that invokes an <see cref="ICommand"/> when the mouse moves over the control
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command",
                typeof(ICommand),
                typeof(MouseMoved),
                new PropertyMetadata(default(ICommand), CommandPropertyChanged));


        private static void CommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Don't do anything if the control doesn't derive from UIElement
            if (!(d is FrameworkElement frameworkElement))
                return;

            // If the value isn't an ICommand derivative 
            if (!(e.NewValue is ICommand))
            {
                Debugger.Break();
                return;
            }

            // Wait for control to initialize
            frameworkElement.Loaded += (sender, initializedEvent) =>
            {
                var mouseInfo = new MouseMovedInfo();

                // Bind MouseLeave event and execute the command
                frameworkElement.PreviewMouseMove += (object sender, MouseEventArgs mouseEvent) =>
                {
                    mouseInfo.LeftButtonPressed = mouseEvent.LeftButton == MouseButtonState.Pressed;
                    mouseInfo.RightButtonPressed = mouseEvent.RightButton == MouseButtonState.Pressed;

                    ((ICommand)e.NewValue).Execute(mouseInfo);
                };
            };
        }
    };


};