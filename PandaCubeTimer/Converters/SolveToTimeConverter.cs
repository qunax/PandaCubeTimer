using System.Globalization;
using PandaCubeTimer.Models;

namespace PandaCubeTimer.Converters;

public class SolveToTimeConverter :  IValueConverter
{
    public const string DefaultFormatWithHours = @"h:\mm\:ss\.ff";
    public const string DefaultFormatWithMinutes = @"m\:ss\.ff";
    public const string DefaultFormatWithoutMinutes = @"s\.ff";
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value == null)
            //Exception?
            return null;
        
        return PuzzleSolveToTimeToDisplay((PuzzleSolve)value, (string?)parameter);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public string PuzzleSolveToTimeToDisplay(PuzzleSolve puzzleSolve, string? formatOverride = null)
    {
        if (puzzleSolve.IsDNF)
            return "DNF";
        
        string plusTwoPenaltyToAdd = puzzleSolve.IsPlusTwo ? "+" : string.Empty;
        
        double seconds = puzzleSolve.SolveTimeSeconds;
        TimeSpan time = TimeSpan.FromSeconds(seconds);
            
        // allow override default formats for customization
        if (formatOverride is not null)
            return time.ToString(formatOverride) +  plusTwoPenaltyToAdd;

        // custom format (if time should display minutes or hours)
        string format;
        if (time.TotalHours > 1)
            format = DefaultFormatWithHours;
        else if(time.TotalMinutes > 1)
            format = DefaultFormatWithMinutes;
        else 
            format = DefaultFormatWithoutMinutes;
            
        return time.ToString(format) + plusTwoPenaltyToAdd;
    }
}