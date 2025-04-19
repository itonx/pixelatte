using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace Pixelatte.UI.Converters
{
    internal class OperationTitleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString())) return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
