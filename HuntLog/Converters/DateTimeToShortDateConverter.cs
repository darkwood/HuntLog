using System;
using System.Globalization;
using HuntLog.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HuntLog.Converters
{
    public class DateTimeToShortDateConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime)value).ToNoString();
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
