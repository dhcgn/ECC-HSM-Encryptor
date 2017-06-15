using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EccHsmEncryptor.Presentation.LookAndFeel.Converter
{
    public class FileSignatureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
                return "no";

            var hasFileSignature = EncryptionSuite.Encryption.FileOperation.HasFileSignature((string) value);
            return hasFileSignature ? "yes" : "no";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}