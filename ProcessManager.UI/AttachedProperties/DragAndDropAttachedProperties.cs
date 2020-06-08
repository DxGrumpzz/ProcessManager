namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// A class that contains attached properties that allow viewmodels to interact with WPF Drag and drop functions
    /// </summary>
    public class DragAndDrop
    {

        #region Drag enter attached property

        public static ICommand GetDragEnterCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DragEnterCommandProperty);
        }

        public static void SetDragEnterCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DragEnterCommandProperty, value);
        }


        /// <summary>
        /// An attached property that invokes an <see cref="ICommand"/> when an element is dragged over the control
        /// </summary>
        public static readonly DependencyProperty DragEnterCommandProperty =
            DependencyProperty.RegisterAttached(
                "DragEnterCommand",
                typeof(ICommand),
                typeof(DragAndDrop),
                new PropertyMetadata(default(ICommand), DragEnterCommandPropertyChanged));


        private static void DragEnterCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement frameworkElement))
            {
                Debugger.Break();
                return;
            };

            // Wait for control to initialize
            frameworkElement.Initialized += (sender, evnt) =>
            {
                // When something is dragged over this control
                frameworkElement.DragEnter += (sender2, evnt2) =>
                {
                    // Execute bound command  
                    ((ICommand)e.NewValue).Execute(frameworkElement.DataContext);
                };
            };
        }


        #endregion


        #region Drag leave attached property

        public static ICommand GetDragLeaveCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DragLeaveCommandProperty);
        }

        public static void SetDragLeaveCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DragLeaveCommandProperty, value);
        }

        /// <summary>
        /// An attached property that will execute an <see cref="ICommand"/> when Dragged data leaves boundries of the control
        /// </summary>
        public static readonly DependencyProperty DragLeaveCommandProperty =
            DependencyProperty.RegisterAttached(
                "DragLeaveCommand",
                typeof(ICommand),
                typeof(DragAndDrop),
                new PropertyMetadata(default(ICommand), DragLeaveCommandPropertyChanged));


        private static void DragLeaveCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement frameworkElement))
            {
                Debugger.Break();
                return;
            };

            // Wait for control to initialize 
            frameworkElement.Initialized += (sender, evnt) =>
            {
                // Bind drag leave event
                frameworkElement.DragLeave += (sender2, evnt2) =>
                {
                    // Execute bound command when DragLeave is invoked
                    ((ICommand)e.NewValue).Execute(frameworkElement.DataContext);
                };
            };
        }

        #endregion


        #region Drag and drop data 

        public static Dictionary<string, object> GetDragDropData(DependencyObject obj)
        {
            return (Dictionary<string, object>)obj.GetValue(DragDropDataProperty);
        }

        public static void SetDragDropData(DependencyObject obj, Dictionary<string, object> value)
        {
            obj.SetValue(DragDropDataProperty, value);
        }

        /// <summary>
        /// An attached property that will contain data for the Drag and Drop operations
        /// </summary>
        public static readonly DependencyProperty DragDropDataProperty =
            DependencyProperty.RegisterAttached(
                "DragDropData",
                typeof(Dictionary<string, object>),
                typeof(DragAndDrop),
                new PropertyMetadata(default(Dictionary<string, object>), DragDropDataPropertyChanged));

        private static void DragDropDataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Make sure the data is a dictionary
            if (!(e.NewValue is Dictionary<string, object>))
            {
                Debugger.Break();
                return;
            };

        }

        #endregion


        #region Start drag drop property

        public static bool GetStartDragDropCommand(DependencyObject obj)
        {
            return (bool)obj.GetValue(StartDragDropCommandProperty);
        }

        public static void SetStartDragDropCommand(DependencyObject obj, bool value)
        {
            obj.SetValue(StartDragDropCommandProperty, value);
        }


        /// <summary>
        /// An attached property that will invoke an <see cref="ICommand"/> when a Drag operation is called
        /// </summary>
        public static readonly DependencyProperty StartDragDropCommandProperty =
            DependencyProperty.RegisterAttached("StartDragDropCommand",
                typeof(bool),
                typeof(DragAndDrop),
                new PropertyMetadata(default(bool), StartDragDropCommandPropertyChanged));


        private static void StartDragDropCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement frameworkElement))
            {
                Debugger.Break();
                return;
            };

            // If drag and drop requested
            if ((bool)e.NewValue == true)
            {
                // Set common drag drop data
                DataObject dataObject = new DataObject();
                dataObject.SetData(DRAG_DROP_DATA_NAME, GetDragDropData(frameworkElement));

                // Start the drag and drop
                DragDrop.DoDragDrop(d, dataObject, DragDropEffects.Move);
            };
        }

        #endregion


        #region Drop attached property

        /// <summary>
        /// Because WPF drag and drop use a <see cref="DataObject"/> which acts like a dictionary we have to specify a common key that will allow data retrieval
        /// </summary>
        private const string DRAG_DROP_DATA_NAME = "Data";

        public static ICommand GetDropCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DropCommandProperty);
        }

        public static void SetDropCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DropCommandProperty, value);
        }

        /// <summary>
        /// An attached property that will execute an <see cref="ICommand"/> when Dragged data is dropped over this control
        /// </summary>
        public static readonly DependencyProperty DropCommandProperty =
            DependencyProperty.RegisterAttached("DropCommand",
                typeof(ICommand),
                typeof(DragAndDrop),
                new PropertyMetadata(default(ICommand), DropCommandPropertyChanged));

        private static void DropCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement frameworkElement))
            {
                Debugger.Break();
                return;
            };

            // Wait for control to initalize
            frameworkElement.Loaded += (sender, evnt) =>
            {
                // Bind drop command
                frameworkElement.Drop += (sender2, dragEvent) =>
                {
                    // Get drag and drop data 
                    var dragDropData = (Dictionary<string, object>)dragEvent.Data.GetData(DRAG_DROP_DATA_NAME);

                    // Exeucte bound commnad
                    ((ICommand)e.NewValue).Execute(dragDropData);
                };
            };
        }

        #endregion

    };
};