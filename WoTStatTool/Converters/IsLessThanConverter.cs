using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WotStatsTool.Converters
{
    public class IsLessThanConverter : IValueConverter
    {
        public static readonly IValueConverter Instance = new IsLessThanConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val;
            if (String.IsNullOrWhiteSpace(value as string))
                val = 0;
            else
                val = double.Parse(value as string, culture.NumberFormat);
            double compareTo = double.Parse(parameter as string, culture.NumberFormat);
            return val < compareTo;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
