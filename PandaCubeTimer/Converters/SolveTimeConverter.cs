using System.Globalization;

namespace PandaCubeTimer.Converters;

public class SolveTimeConverter : IValueConverter
{
    public const string DefaultFormatWithHours = @"h:\mm\:ss\.ff";
    public const string DefaultFormatWithMinutes = @"m\:ss\.ff";
    public const string DefaultFormatWithoutMinutes = @"s\.ff";
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            
            // allow override default formats for customization
            if (parameter is string formatOverride)
                return time.ToString(formatOverride);

            // custom format depending if time should display minutes or hours
            string format;
            if (time.TotalHours > 1)
                format = DefaultFormatWithHours;
            else if(time.TotalMinutes > 1)
                format = DefaultFormatWithMinutes;
            else 
                format = DefaultFormatWithoutMinutes;
            
            return time.ToString(format);
        }

        return "Error";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}