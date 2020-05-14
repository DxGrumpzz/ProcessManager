namespace ProcessManager.UI
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// 
    /// </summary>
    public class ProcessTypeToImageValueConverter : BaseValueConverter<ProcessTypeToImageValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ProcessType)value)
            {
                case ProcessType.Console:
                {
                    return App.Current.Resources.GetConsoleProcessImage();
                };

                case ProcessType.GUI:
                {
                    return App.Current.Resources.GetGUIProcessImage();
                };

                default:
                    Debugger.Break();
                    return null;
            };
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    };
};