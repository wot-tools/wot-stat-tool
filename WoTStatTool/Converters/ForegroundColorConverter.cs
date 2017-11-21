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
    public class ForegroundColorConverter : IValueConverter
    {
        public static readonly ForegroundColorConverter Instance = new ForegroundColorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == value)
                return Brushes.Black;

            Color color;

            switch (value)
            {
                case SolidColorBrush brush: color = brush.Color; break;
                case System.Windows.Controls.DataGridCell cell: color = ((SolidColorBrush)cell.Background)?.Color ?? Colors.White; break;
                default: throw new NotImplementedException();
            }

            //https://stackoverflow.com/questions/1855884/determine-font-color-based-on-background-color
            double a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;

            return a < 0.5 ? Brushes.Black : Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
