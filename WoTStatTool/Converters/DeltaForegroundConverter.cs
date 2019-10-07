using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WotStatsTool.Converters
{
    public class DeltaForegroundConverter : IValueConverter
    {
        public static readonly IValueConverter Instance = new DeltaForegroundConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double val))
                return Brushes.Gray;

            return val switch
            {
                _ when val > 0 => Brushes.MediumSeaGreen,
                _ when val < 0 => Brushes.Tomato,
                _ => Brushes.Gray,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
