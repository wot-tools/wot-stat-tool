using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WotStatsTool.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public static readonly DateTimeConverter Instance = new DateTimeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case DateTime d: return d.Ticks;
                //case string s: return DateTime.Parse(s).Ticks;
                default: throw new ArgumentException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case long l: return new DateTime(l);
                //case string s: return new DateTime(long.Parse(s));
                default: throw new ArgumentException();
            }
        }
    }
}
