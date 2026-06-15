using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System;
using System.Globalization;
using System.IO;

namespace VendingMachines.Converters
{
    public class Base64ImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string base64 && !string.IsNullOrWhiteSpace(base64))
            {
                try
                {
                    string base64Data = base64;
                    int commaIndex = base64.IndexOf(',');
                    if (commaIndex >= 0)
                        base64Data = base64.Substring(commaIndex + 1);

                    byte[] bytes = System.Convert.FromBase64String(base64Data);
                    using (var ms = new MemoryStream(bytes))
                    {
                        return new Bitmap(ms);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка преобразования изображения: {ex.Message}");
                    return null;
                }
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}