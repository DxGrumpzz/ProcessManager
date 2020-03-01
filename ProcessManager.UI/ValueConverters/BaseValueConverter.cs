namespace ProcessManager.UI
{
    using System;
    using System.Windows.Data;
    using System.Globalization;
    using System.Windows.Markup;

    /// <summary>
    /// A base ValueConverter for all <see cref="IValueConverter"/> classes to inherit.
	/// Allows for a more simple implmentation in Xaml
    /// </summary>
    /// <typeparam name="T"> The type of class or ValueConverter </typeparam>
    abstract public class BaseValueConverter<T> : MarkupExtension, IValueConverter
        where T : class, new()
    {

        /// <summary>
        /// A single static instance of this value converter
        /// </summary>
        private static T Converter = null;


        /// <summary>
        /// Converts a value into another value with a possibility to pass a specifc value with the parameter
        /// </summary>
        /// <param name="value"> The value to convert </param>
        /// <param name="targetType"></param>
        /// <param name="parameter"> </param>
        /// <param name="culture"></param>
        /// <returns> Returns an <see cref="object"/> that can be cast into any other object </returns>
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);


        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);


        /// <summary>
        /// Provides a static instance of the value converter 
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // If the converter instance is null create a new type of what ever type passed in the type argument
            return Converter ?? (Converter = new T());
        }
    }
}
