using System.Globalization;

namespace PandaCubeTimer.Converters;

public class SolveTimeConverter : IValueConverter
{
    public const string DefaultFormatWithHours = @"h:\mm\:ss\.ff";
    public const string DefaultFormatWithMinutes = @"m\:ss\.ff";
    public const string DefaultFormatWithoutMinutes = @"s\.ff";
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return DoubleToStringSeconds((double?)value, (string?)parameter);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// converts double value to formatted string typo h:\mm\:ss\.ff
    /// (shows hours/minutes only if they are present)
    /// </summary>
    /// <param name="value"></param>
    /// <param name="formatOverride"></param>
    /// <returns>Formatted String or "Error"</returns>
    public string? DoubleToStringSeconds(double? value, string? formatOverride = null)
    {
        if (value is null)
            return null;
            // return "Error";
        
        double seconds = value.Value;
        TimeSpan time = TimeSpan.FromSeconds(seconds);
            
        // allow override default formats for customization
        if (formatOverride is not null)
            return time.ToString(formatOverride);

        // custom format (if time should display minutes or hours)
        string format;
        if (time.TotalHours > 1)
            format = DefaultFormatWithHours;
        else if(time.TotalMinutes > 1)
            format = DefaultFormatWithMinutes;
        else 
            format = DefaultFormatWithoutMinutes;
            
        return time.ToString(format);
    }
}