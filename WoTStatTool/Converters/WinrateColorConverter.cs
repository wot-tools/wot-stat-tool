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
            if (!(value is double val))
                val = 0;

            static SolidColorBrush fromHex(WinrateColors color) => color == WinrateColors.None ? null :
                new SolidColorBrush(Color.FromRgb((byte)((int)color >> 16), (byte)((int)color >> 8), (byte)color));

            return fromHex(val switch
            {
                _ when double.IsNaN(val) => WinrateColors.None,
                _ when val < 0.46 => WinrateColors.VeryBad,
                _ when val < 0.47 => WinrateColors.Bad,
                _ when val < 0.48 => WinrateColors.BelowAverage,
                _ when val < 0.50 => WinrateColors.Average,
                _ when val < 0.52 => WinrateColors.AboveAverage,
                _ when val < 0.54 => WinrateColors.Good,
                _ when val < 0.56 => WinrateColors.VeryGood,
                _ when val < 0.60 => WinrateColors.Great,
                _ when val < 0.65 => WinrateColors.Unicum,
                _ => WinrateColors.SuperUnicum,
            });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
