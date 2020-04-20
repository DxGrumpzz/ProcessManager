namespace ProcessManager.UI
{
    using System;
    using System.Globalization;
    using System.Windows;


    /// <summary>
    /// 
    /// </summary>
    public class BooleanToVisibilityValueConverter : BaseValueConverter<BooleanToVisibilityValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If there is an argument
            if (parameter != null)
            {
                // Try and convert to a boolean, if it fails will always return false
                bool.TryParse(parameter as string, out bool invert);

                if (invert == true)
                {
                    return !System.Convert.ToBoolean(value) == true ? Visibility.Visible : Visibility.Hidden;
                };
            };

            return System.Convert.ToBoolean(value) == true ? Visibility.Visible : Visibility.Hidden;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    };
};
