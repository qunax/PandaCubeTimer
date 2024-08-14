namespace PandaCubeTimer.Models;

public class Solve
{
    public string Time { get; set; }
    public bool IsPlusTwo { get; set; }
    public bool IsDNF { get; set; }
    public string Scramble { get; set; }
    public string Discipline { get; set; }
    public string Session { get; set; }
    public DateTime DateTime { get; set; }
    public string? Comment { get; set; }
    // public Image ScrambledCube { get; set; }
}