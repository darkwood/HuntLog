using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HuntLog.Converters
{
    public class DateTimeToShortDateAndTimeConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = ((DateTime)value);
            if(d == DateTime.MaxValue)
            {
                return string.Empty;
            }

            return d.ToString("hh:mm", new CultureInfo("nb-no"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
