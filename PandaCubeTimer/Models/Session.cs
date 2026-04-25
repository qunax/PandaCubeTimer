using SQLite;

namespace PandaCubeTimer.Models;

public class Session
{
    [PrimaryKey] public Guid Id { get; set; } = Guid.NewGuid();

    public string SessionName { get; set; } = null!;

    [Ignore]
    public ICollection<PuzzleSolve> Solves { get; set; } = null!;
}