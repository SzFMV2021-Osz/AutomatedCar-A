namespace AutomatedCar.Visualization
{
    using System;
    using System.Globalization;
    using Avalonia.Data.Converters;

    public class BoolToStatusConverter : IValueConverter
    {
        const string on = "ON";
        const string off = "OFF";

        public static BoolToStatusConverter Instance { get; } = new BoolToStatusConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? on : off;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
