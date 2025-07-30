using System.Globalization;
using PandaCubeTimer.Models;

namespace PandaCubeTimer.Converters;

public class InspectionPenaltyToAfterSolvePenaltyVisibilityConverter : IValueConverter
{
    /// <summary>
    /// converts penalties from solve to visibilities 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return false;
        
        if(parameter is null)
            throw new ArgumentNullException(nameof(parameter));
        
        PuzzleSolve puzzleSolve = (PuzzleSolve)value;
        SolvePenalty inspectionPenalty = (SolvePenalty)parameter;
        
        // no penalty on inspection => all penalties are available to apply after solve is done
        if (puzzleSolve is { IsPlusTwo: false, IsDNF: false })
        {
            return true;
        }
        
        if (puzzleSolve.IsDNF)
        {
            // after DNF on inspection solve can be only deleted
            if(inspectionPenalty == SolvePenalty.Delete)
                return true;
            
            return false;
        }

        if (puzzleSolve.IsPlusTwo)
        {
            // if (inspectionPenalty == SolvePenalty.NoPenalty)
            //     return false;
            return true;
        }

        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}