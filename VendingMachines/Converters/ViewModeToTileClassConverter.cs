using Avalonia.Data.Converters;
using System;
using System.Globalization;
using VendingMachines.ViewModels;

namespace VendingMachines.Converters
{
    public class ViewModeToTileClassConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is ViewMode mode)
            {
                return mode == ViewMode.Tile ? "TileActive" : "TileInactive";
            }
            return "TileInactive";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}