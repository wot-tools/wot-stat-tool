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
    public class WinrateColorConverter : IValueConverter
    {
        public static readonly IValueConverter Instance = new WinrateColorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = (double)value;

            Func<int, SolidColorBrush> fromHex = hex =>
            {
                return new SolidColorBrush(Color.FromRgb((byte)(hex >> 16), (byte)(hex >> 8), (byte)hex));
            };

            switch (val)
            {
                case double testVal when double.IsNaN(testVal): return null;
                case double testVal when testVal < 0.46: return fromHex((int)WinrateColors.VeryBad);
                case double testVal when testVal < 0.47: return fromHex((int)WinrateColors.Bad);
                case double testVal when testVal < 0.48: return fromHex((int)WinrateColors.BelowAverage);
                case double testVal when testVal < 0.50: return fromHex((int)WinrateColors.Average);
                case double testVal when testVal < 0.52: return fromHex((int)WinrateColors.AboveAverage);
                case double testVal when testVal < 0.54: return fromHex((int)WinrateColors.Good);
                case double testVal when testVal < 0.56: return fromHex((int)WinrateColors.VeryGood);
                case double testVal when testVal < 0.60: return fromHex((int)WinrateColors.Great);
                case double testVal when testVal < 0.65: return fromHex((int)WinrateColors.Unicum);
                case double testVal: return fromHex((int)WinrateColors.SuperUnicum);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
