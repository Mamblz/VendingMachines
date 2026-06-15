using Avalonia.Data.Converters;
using System;
using System.Globalization;
using VendingMachines.ViewModels;

namespace VendingMachines.Converters
{
    public class ViewModeToTableClassConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is ViewMode mode)
            {
                return mode == ViewMode.Table ? "TableActive" : "TableInactive";
            }
            return "TableInactive";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}