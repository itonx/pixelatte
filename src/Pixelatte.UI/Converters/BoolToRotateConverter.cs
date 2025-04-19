using Microsoft.UI.Xaml.Data;
using System;

namespace Pixelatte.UI.Converters
{
    internal class BoolToRotateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value) return 90;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
