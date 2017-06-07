using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace EccHsmEncryptor.Presentation.LookAndFeel.Converter
{
    public class FileSizeHumanReadableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var filePath = value as string;
            if (filePath == null)
                return "-";

            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                return "file not found";
            }
            var mb = (double)fileInfo.Length/1024/1014;
            return $"{mb:N3}MB";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}