namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// 
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

            frameworkElement.Initialized += (sender, evnt) =>
            {
                frameworkElement.DragEnter += (sender2, evnt2) =>
                {
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

            frameworkElement.Initialized += (sender, evnt) =>
            {
                frameworkElement.DragLeave += (sender2, evnt2) =>
                {
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

        public static readonly DependencyProperty DragDropDataProperty =
            DependencyProperty.RegisterAttached(
                "DragDropData",
                typeof(Dictionary<string, object>),
                typeof(DragAndDrop),
                new PropertyMetadata(default(Dictionary<string, object>), DragDropDataPropertyChanged));

        private static void DragDropDataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is Dictionary<string, object>))
            {
                Debugger.Break();
                return;
            };

        }

        #endregion


        #region Drop attached property

        private const string DRAG_DROP_DATA_NAME = "Data";

        public static ICommand GetDropCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DropCommandProperty);
        }

        public static void SetDropCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DropCommandProperty, value);
        }

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

            frameworkElement.Initialized += (sender, evnt) =>
            {
                frameworkElement.Drop += (sender2, dragEvent) =>
                {
                    var dragDropData = (Dictionary<string, object>)dragEvent.Data.GetData(DRAG_DROP_DATA_NAME);

                    ((ICommand)e.NewValue).Execute(dragDropData);
                };
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

            if ((bool)e.NewValue == true)
            {
                DataObject dataObject = new DataObject();
                dataObject.SetData(DRAG_DROP_DATA_NAME, GetDragDropData(frameworkElement));

                DragDrop.DoDragDrop(d, dataObject, DragDropEffects.Move);
            };
        }

        #endregion

    };
};