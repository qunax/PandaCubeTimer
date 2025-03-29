using System.ComponentModel.DataAnnotations;

namespace PandaCubeTimer.Models;

public class Session
{
    [Key]
    public int Id { get; set; }
    public string SessionName { get; set; } = null!;
    public ICollection<PuzzleSolve> Solves { get; set; } = null!;
}