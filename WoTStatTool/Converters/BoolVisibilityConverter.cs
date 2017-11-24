using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WotStatsTool.Converters
{
    public class BoolVisibilityConverter : IValueConverter
    {
        public static readonly BoolVisibilityConverter Instance = new BoolVisibilityConverter();

        /// <param name="parameter">if true inverts value</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool p = parameter is default ? false : bool.Parse(parameter as string);
            if (value is bool v)
                return v ^ p ? Visibility.Visible : Visibility.Collapsed;
            else
                throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
