using System.Globalization;

namespace PandaCubeTimer.Converters;

public class InspectionPenaltyTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value == null)
            throw new ArgumentNullException(nameof(value));
        
        int remainedTicks = (int)value;
        if (remainedTicks > 0)
        {
            return remainedTicks.ToString();
        }
        if (remainedTicks > -3 && remainedTicks <= 0)
        {
            return "+2";
        }
        return "DNF";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}