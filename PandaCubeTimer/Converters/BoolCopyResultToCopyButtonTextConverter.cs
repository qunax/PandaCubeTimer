using System.Globalization;

namespace PandaCubeTimer.Converters;

public class BoolCopyResultToCopyButtonTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return "Copy";
        
        return (bool)value ? "Copied!" : "Failed to copy.";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}