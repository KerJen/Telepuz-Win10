using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telepuz.Helpers.Converters
{
    public class BooleanToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) =>
            (bool)value ^ (parameter as string ?? string.Empty).Equals("Reverse") ?
                1f : 0f;

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            (float)value == 1f ^ (parameter as string ?? string.Empty).Equals("Reverse");
    }
}
