using System.Globalization;

namespace PandaCubeTimer.Converters;

public class BoolCopyResultToCopyButtonColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return Colors.CornflowerBlue;
        
        return (bool)value ? Colors.LightSeaGreen : Colors.Red;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}