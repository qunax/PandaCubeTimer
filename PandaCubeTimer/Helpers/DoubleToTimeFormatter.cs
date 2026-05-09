namespace PandaCubeTimer.Helpers;

public static class DoubleToTimeFormatter
{
    public static string FormatTime(this double totalSeconds)
    {
        if (totalSeconds < 60)
        {
            return $"{totalSeconds:0.000}"; 
        }
    
        TimeSpan ts = TimeSpan.FromSeconds(totalSeconds);
    
        return $"{(int)ts.TotalMinutes}:{ts.Seconds:D2}.{ts.Milliseconds / 100}";
    }
        //=> TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss\.ff");
}