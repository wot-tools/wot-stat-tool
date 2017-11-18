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
    public class Wn8ForegroundColorConverter : IValueConverter
    {
        public static readonly IValueConverter Instance = new Wn8ForegroundColorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val;
            if (String.IsNullOrWhiteSpace(value as string))
                val = 0;
            else
                val = double.Parse(value as string, culture.NumberFormat);


            switch (val)
            {
                case double testVal when double.IsNaN(testVal): return Brushes.Black;
                case double testVal when testVal < 300: return Brushes.White;
                case double testVal when testVal < 450: return Brushes.White;
                case double testVal when testVal < 650: return Brushes.White;
                case double testVal when testVal < 900: return Brushes.Black;
                case double testVal when testVal < 1200: return Brushes.Black;
                case double testVal when testVal < 1600: return Brushes.White;
                case double testVal when testVal < 2000: return Brushes.Black;
                case double testVal when testVal < 2450: return Brushes.White;
                case double testVal when testVal < 2900: return Brushes.White;
                case double testVal: return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
