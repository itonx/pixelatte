using Microsoft.UI.Xaml.Data;
using System;

namespace Pixelatte.UI.Converters
{
    internal class IsLocalServerRunningToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value) return "\uE71A";

            return "\uE768";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
