namespace ProcessManager.UI
{
    using System.Windows;

    /// <summary>
    /// An attached property that is used in BuildResourceExtentions project 
    /// </summary>
    public class BuildResourceExtentions
    {

        public static bool GetLoadIntoFile(DependencyObject obj)
        {
            return (bool)obj.GetValue(LoadIntoFileAttachedProperty);
        }

        public static void SetLoadIntoFile(DependencyObject obj, bool value)
        {
            obj.SetValue(LoadIntoFileAttachedProperty, value);
        }

        public static readonly DependencyProperty LoadIntoFileAttachedProperty =
            DependencyProperty.RegisterAttached("LoadIntoFile", typeof(bool), typeof(BuildResourceExtentions), new PropertyMetadata(false));

    };
};