namespace PandaCubeTimer.Helpers;

public static class DoubleToTimeFormatter
{
    public static string FormatTime(this double seconds) 
        => TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss\.ff");
}