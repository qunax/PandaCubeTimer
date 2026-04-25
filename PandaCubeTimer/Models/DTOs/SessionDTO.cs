namespace PandaCubeTimer.Models.DTOs;

public class SessionDTO : Session
{
    public string? DisciplineName { get; set; }

    public ICollection<PuzzleSolve>? Solves { get; set; }
}