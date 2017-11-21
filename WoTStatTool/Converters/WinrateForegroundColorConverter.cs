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
    public class WinrateForegroundColorConverter : IValueConverter
    {
        public static readonly IValueConverter Instance = new WinrateForegroundColorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = (double)value;

            switch (val)
            {
                case double testVal when double.IsNaN(testVal): return Brushes.Black;
                case double testVal when testVal < 0.46: return Brushes.White;
                case double testVal when testVal < 0.47: return Brushes.White;
                case double testVal when testVal < 0.48: return Brushes.White;
                case double testVal when testVal < 0.50: return Brushes.Black;
                case double testVal when testVal < 0.52: return Brushes.Black;
                case double testVal when testVal < 0.54: return Brushes.White;
                case double testVal when testVal < 0.56: return Brushes.Black;
                case double testVal when testVal < 0.60: return Brushes.White;
                case double testVal when testVal < 0.65: return Brushes.White;
                case double testVal: return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
