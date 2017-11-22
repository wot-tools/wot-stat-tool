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
            double val = (double)value;

            switch (val)
            {
                case double testVal when testVal > 0: return Brushes.MediumSeaGreen;
                case double testVal when testVal < 0: return Brushes.Tomato;
                case double testVal: return Brushes.Gray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
