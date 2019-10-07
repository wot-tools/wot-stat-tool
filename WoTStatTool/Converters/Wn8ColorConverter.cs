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
    public class Wn8ColorConverter : IValueConverter
    {
        public static readonly IValueConverter Instance = new Wn8ColorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = (double)value;

            static SolidColorBrush fromHex(Wn8Colors color) => color == Wn8Colors.None ? null :
                new SolidColorBrush(Color.FromRgb((byte)((int)color >> 16), (byte)((int)color >> 8), (byte)color));

            return fromHex(val switch
            {
                _ when double.IsNaN(val) => Wn8Colors.None,
                _ when val < 300 => Wn8Colors.VeryBad,
                _ when val < 450 => Wn8Colors.Bad,
                _ when val < 650 => Wn8Colors.BelowAverage,
                _ when val < 900 => Wn8Colors.Average,
                _ when val < 1200 => Wn8Colors.AboveAverage,
                _ when val < 1600 => Wn8Colors.Good,
                _ when val < 2000 => Wn8Colors.VeryGood,
                _ when val < 2450 => Wn8Colors.Great,
                _ when val < 2900 => Wn8Colors.Unicum,
                _ => Wn8Colors.SuperUnicum,
            });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
