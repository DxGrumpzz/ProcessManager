namespace ProcessManager.UI
{
    using System;
    using System.Diagnostics;
    using System.Globalization;


    /// <summary>
    /// A value converter that takes a <see cref="ProcessType"/> and returns a string based on it's value
    /// </summary>
    public class ProcessTypeToTextConverter : BaseValueConverter<ProcessTypeToTextConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ProcessType)value)
            {
                case ProcessType.Console:
                    return "Console process";

                case ProcessType.GUI:
                    return "GUI process";

                default:
                {
                    Debugger.Break();
                    return null;
                };
            };
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    };
};