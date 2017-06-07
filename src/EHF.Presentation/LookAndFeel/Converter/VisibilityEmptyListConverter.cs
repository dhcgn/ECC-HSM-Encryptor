using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EccHsmEncryptor.Presentation.LookAndFeel.Converter
{
    public class VisibilityEmptyListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;

            var list = value as IEnumerable;
            var moveNext = list.GetEnumerator().MoveNext();

            return moveNext ? Visibility.Collapsed :Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}