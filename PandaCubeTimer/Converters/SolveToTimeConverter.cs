using System.Globalization;
using PandaCubeTimer.Helpers;
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
        return puzzleSolve.SolveTimeSeconds.FormatTime() + plusTwoPenaltyToAdd;
    }
}