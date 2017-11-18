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
            double val;
            if (String.IsNullOrWhiteSpace(value as string))
                val = 0;
            else
                val = double.Parse(value as string, culture.NumberFormat);

            Func<int, SolidColorBrush> fromHex = hex =>
            {
                return new SolidColorBrush(Color.FromRgb((byte)(hex >> 16), (byte)(hex >> 8), (byte)hex));
            };

            switch (val)
            {
                case double testVal when double.IsNaN(testVal): return null;
                case double testVal when testVal < 300: return fromHex((int)Wn8Colors.VeryBad);
                case double testVal when testVal < 450: return fromHex((int)Wn8Colors.Bad);
                case double testVal when testVal < 650: return fromHex((int)Wn8Colors.BelowAverage);
                case double testVal when testVal < 900: return fromHex((int)Wn8Colors.Average);
                case double testVal when testVal < 1200: return fromHex((int)Wn8Colors.AboveAverage);
                case double testVal when testVal < 1600: return fromHex((int)Wn8Colors.Good);
                case double testVal when testVal < 2000: return fromHex((int)Wn8Colors.VeryGood);
                case double testVal when testVal < 2450: return fromHex((int)Wn8Colors.Great);
                case double testVal when testVal < 2900: return fromHex((int)Wn8Colors.Unicum);
                case double testVal: return fromHex((int)Wn8Colors.SuperUnicum);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
